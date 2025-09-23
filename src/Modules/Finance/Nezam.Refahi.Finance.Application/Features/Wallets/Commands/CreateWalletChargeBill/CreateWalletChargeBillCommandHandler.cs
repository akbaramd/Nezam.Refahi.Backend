using FluentValidation;
using MediatR;
using Nezam.Refahi.Finance.Application.Services;
using Nezam.Refahi.Finance.Contracts.Commands.Bills;
using Nezam.Refahi.Finance.Contracts.Dtos;
using Nezam.Refahi.Finance.Domain.Entities;
using Nezam.Refahi.Finance.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Finance.Application.Features.Wallets.Commands.CreateWalletChargeBill;

/// <summary>
/// Handler for CreateBillChargeBillCommand - Creates a bill for charging a wallet
/// </summary>
public class CreateWalletChargeBillCommandHandler : IRequestHandler<CreateBillChargeBillCommand, ApplicationResult<CreateBillChargeBillResponse>>
{
    private readonly IBillRepository _billRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly IWalletDepositRepository _walletDepositRepository;
    private readonly IValidator<CreateBillChargeBillCommand> _validator;
    private readonly IFinanceUnitOfWork _unitOfWork;

    public CreateWalletChargeBillCommandHandler(
        IBillRepository billRepository,
        IWalletRepository walletRepository,
        IWalletDepositRepository walletDepositRepository,
        IValidator<CreateBillChargeBillCommand> validator,
        IFinanceUnitOfWork unitOfWork)
    {
        _billRepository = billRepository ?? throw new ArgumentNullException(nameof(billRepository));
        _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
        _walletDepositRepository = walletDepositRepository ?? throw new ArgumentNullException(nameof(walletDepositRepository));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<ApplicationResult<CreateBillChargeBillResponse>> Handle(
        CreateBillChargeBillCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Validate the command
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return ApplicationResult<CreateBillChargeBillResponse>.Failure(
                    validationResult.Errors.Select(e => e.ErrorMessage).ToList(),
                    "Validation failed"
                );
            }

            // Check if wallet exists for this user
            var wallet = await _walletRepository.GetByNationalNumberAsync(
                request.UserNationalNumber, cancellationToken);
            
            if (wallet == null)
            {
                return ApplicationResult<CreateBillChargeBillResponse>.Failure(
                    "Wallet not found for this user");
            }

            // Check if wallet is active
            if (wallet.Status != Domain.Enums.WalletStatus.Active)
            {
                return ApplicationResult<CreateBillChargeBillResponse>.Failure(
                    $"Cannot create charge bill. Wallet status: {wallet.Status}");
            }

            // Create wallet deposit first
            var chargeAmount = Money.FromRials(request.AmountRials);
            var walletDeposit = new WalletDeposit(
                walletId: wallet.Id,
                userNationalNumber: request.UserNationalNumber,
                amount: chargeAmount,
                description: request.Description ?? "Wallet charging deposit",
                externalReference: null,
                metadata: request.Metadata);

            // Add deposit to repository
            await _walletDepositRepository.AddAsync(walletDeposit, cancellationToken: cancellationToken);

            // Use deposit ID as reference for the bill
            var referenceId = walletDeposit.Id.ToString();

            // Create bill for wallet charge
            var bill = new Bill(
                title: $"Wallet Deposit - {request.UserFullName}",
                referenceId: referenceId,
                billType: "WalletDeposit",
                userNationalNumber: request.UserNationalNumber,
                userFullName: request.UserFullName,
                description: request.Description ?? "Wallet depositing bill",
                dueDate: DateTime.UtcNow.AddDays(7), // 7 days to pay
                metadata: request.Metadata);

            // Add bill item for the charge amount
            bill.AddItem(
                title: "Wallet Deposit Amount",
                description: $"Deposit wallet with {request.AmountRials:N0} rials",
                unitPrice: chargeAmount,
                quantity: 1,
                discountPercentage: 0);

            // Issue the bill
            bill.Issue();

            // Save bill
            await _billRepository.AddAsync(bill, cancellationToken:cancellationToken);
            
            // Set bill ID on the deposit
            walletDeposit.SetBillId(bill.Id);
            await _walletDepositRepository.UpdateAsync(walletDeposit, cancellationToken: cancellationToken);
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Return response
            var response = new CreateBillChargeBillResponse
            {
                BillId = bill.Id,
                BillNumber = bill.BillNumber,
                UserNationalNumber = request.UserNationalNumber,
                UserFullName = request.UserFullName,
                AmountRials = request.AmountRials,
                Status = bill.Status.ToString(),
                IssueDate = bill.IssueDate,
                ReferenceId = referenceId,
                DepositId = walletDeposit.Id
            };

            return ApplicationResult<CreateBillChargeBillResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return ApplicationResult<CreateBillChargeBillResponse>.Failure(
                ex,
                "Failed to create wallet charge bill"
            );
        }
    }
}
