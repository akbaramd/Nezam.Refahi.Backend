using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MassTransit;
using Nezam.Refahi.Contracts.Finance.v1.Messages;
using Nezam.Refahi.Finance.Application.Services;
using Nezam.Refahi.Finance.Application.Commands.Wallets;
using Nezam.Refahi.Finance.Domain.Entities;
using Nezam.Refahi.Finance.Domain.Enums;
using Nezam.Refahi.Finance.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Finance.Application.Features.Wallets.Commands.CreateWalletDeposit;

/// <summary>
/// Handler for CreateWalletDepositCommand - Creates a wallet deposit and generates a bill for payment
/// </summary>
public class CreateWalletDepositCommandHandler : IRequestHandler<CreateWalletDepositCommand, ApplicationResult<CreateWalletDepositResponse>>
{
    private readonly IBillRepository _billRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly IWalletDepositRepository _walletDepositRepository;
    private readonly IValidator<CreateWalletDepositCommand> _validator;
    private readonly IFinanceUnitOfWork _unitOfWork;
    private readonly IBus _publishEndpoint;

    public CreateWalletDepositCommandHandler(
        IBillRepository billRepository,
        IWalletRepository walletRepository,
        IWalletDepositRepository walletDepositRepository,
        IValidator<CreateWalletDepositCommand> validator,
        IFinanceUnitOfWork unitOfWork,
        IBus publishEndpoint)
    {
        _billRepository = billRepository ?? throw new ArgumentNullException(nameof(billRepository));
        _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
        _walletDepositRepository = walletDepositRepository ?? throw new ArgumentNullException(nameof(walletDepositRepository));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
    }

    public async Task<ApplicationResult<CreateWalletDepositResponse>> Handle(
        CreateWalletDepositCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Validate the command
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return ApplicationResult<CreateWalletDepositResponse>.Failure(
                    validationResult.Errors.Select(e => e.ErrorMessage).ToList(),
                    "Validation failed"
                );
            }

            // Check if wallet exists for this user, create if not found
            var wallet = await _walletRepository.GetByExternalUserIdAsync(
                request.ExternalUserId, cancellationToken);
            
            if (wallet == null)
            {
                // Auto-create wallet if not found
                wallet = new Wallet(
                    externalUserId: request.ExternalUserId,
                    walletName: $"کیف پول {request.UserFullName}",
                    description: "کیف پول پیش‌فرض کاربر",
                    metadata: new Dictionary<string, string>
                    {
                        { "created_by", "system" },
                        { "created_reason", "auto_created_on_deposit_request" }
                    });

                await _walletRepository.AddAsync(wallet, cancellationToken: cancellationToken);
            }

            // Check if wallet is active
            if (wallet.Status != Domain.Enums.WalletStatus.Active)
            {
                return ApplicationResult<CreateWalletDepositResponse>.Failure(
                    $"امکان ایجاد واریز وجود ندارد. وضعیت کیف پول: {wallet.Status}");
            }

            // Validate deposit amount
            if (request.AmountRials <= 0)
            {
                return ApplicationResult<CreateWalletDepositResponse>.Failure(
                    "مبلغ واریز باید مثبت باشد");
            }

            // Check for duplicate external reference if provided
            if (!string.IsNullOrEmpty(request.ExternalReference))
            {
                var existingDeposit = await _walletDepositRepository.GetByExternalReferenceAsync(
                    request.ExternalReference, cancellationToken);
                
                if (existingDeposit != null)
                {
                    return ApplicationResult<CreateWalletDepositResponse>.Failure(
                        "درخواست واریز با این شناسه خارجی قبلاً ثبت شده است");
                }
            }

            // Create wallet deposit first
            var depositAmount = Money.FromRials(request.AmountRials);
            
            try
            {
                // Create wallet deposit first
                var walletDeposit = new WalletDeposit(
                    walletId: wallet.Id,
                    externalUserId: request.ExternalUserId,
                    amount: depositAmount,
                    description: request.Description ?? "درخواست واریز کیف پول",
                    externalReference: request.ExternalReference,
                    metadata: request.Metadata);

                // Move to processing while orchestrator prepares bill
                walletDeposit.StartProcessing();

                await _walletDepositRepository.AddAsync(walletDeposit, cancellationToken: cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // Publish integration event for orchestrator to create bill
                var depositRequestedEvent = new WalletDepositRequestedEventMessage()
                {
                    ExternalUserId = request.ExternalUserId,
                    UserFullName = request.UserFullName,
                    AmountRials = request.AmountRials,
                    TrackingCode = walletDeposit.TrackingCode,
                    Currency = "IRR",
                    Description = request.Description,
                    WalletDepositId = walletDeposit.Id,
                    Metadata = request.Metadata ?? new Dictionary<string, string>()
                };
                await _publishEndpoint.Publish(depositRequestedEvent, cancellationToken);

                // Return response with deposit info; bill to be created asynchronously
                var response = new CreateWalletDepositResponse
                {
                    DepositId = walletDeposit.Id,
                    BillId = Guid.Empty,
                    BillNumber = string.Empty,
                    UserExternalUserId = request.ExternalUserId,
                    UserFullName = request.UserFullName,
                    AmountRials = request.AmountRials,
                    DepositStatus = walletDeposit.Status.ToString(),
                    DepositStatusText = GetWalletDepositStatusText(walletDeposit.Status),
                    BillStatus = "Pending",
                    BillStatusText = GetBillStatusText(Domain.Enums.BillStatus.Draft),
                    RequestedAt = walletDeposit.RequestedAt,
                    BillIssueDate = null,
                    ReferenceId = walletDeposit.TrackingCode
                };

                return ApplicationResult<CreateWalletDepositResponse>.Success(response, "درخواست واریز ثبت شد. صورت‌حساب به‌زودی ایجاد می‌شود.");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return ApplicationResult<CreateWalletDepositResponse>.Failure(
                    ex,
                    "خطا در ایجاد واریز کیف پول - تداخل در داده‌ها. لطفاً مجدداً تلاش کنید");
            }
            catch (DbUpdateException ex)
            {
                return ApplicationResult<CreateWalletDepositResponse>.Failure(
                    ex,
                    "خطا در ذخیره اطلاعات واریز کیف پول");
            }
        }
        catch (Exception ex)
        {
            return ApplicationResult<CreateWalletDepositResponse>.Failure(
                ex,
                "خطا در ایجاد واریز کیف پول"
            );
        }
    }

    /// <summary>
    /// Convert wallet deposit status to Persian text
    /// </summary>
    private static string GetWalletDepositStatusText(WalletDepositStatus status)
    {
        return status switch
        {
            WalletDepositStatus.Pending => "در انتظار",
            WalletDepositStatus.Completed => "تکمیل شده",
            WalletDepositStatus.Failed => "ناموفق",
            WalletDepositStatus.Cancelled => "لغو شده",
            _ => status.ToString()
        };
    }

    /// <summary>
    /// Convert bill status to Persian text
    /// </summary>
    private static string GetBillStatusText(BillStatus status)
    {
        return status switch
        {
            BillStatus.Draft => "پیش‌نویس",
            BillStatus.Issued => "صادر شده",
            BillStatus.PartiallyPaid => "پرداخت جزئی",
            BillStatus.FullyPaid => "پرداخت کامل",
            BillStatus.Overdue => "منقضی شده",
            BillStatus.Cancelled => "لغو شده",
            _ => status.ToString()
        };
    }
}
