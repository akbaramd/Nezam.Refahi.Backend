using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Finance.Application.Services;
using Nezam.Refahi.Finance.Application.Commands.Payments;
using Nezam.Refahi.Finance.Application.Commands.Wallets;
using Nezam.Refahi.Finance.Domain.Enums;
using Nezam.Refahi.Finance.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Interfaces;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Finance.Application.Features.Payments.Commands.PayWithWallet;

/// <summary>
/// Handler for PayWithWalletCommand - Pays a bill using wallet balance
/// </summary>
public class PayWithWalletCommandHandler : IRequestHandler<PayWithWalletCommand, ApplicationResult<PayWithWalletResponse>>
{
    private readonly IBillRepository _billRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IValidator<PayWithWalletCommand> _validator;
    private readonly IFinanceUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    public PayWithWalletCommandHandler(
        IBillRepository billRepository,
        IWalletRepository walletRepository,
        IPaymentRepository paymentRepository,
        IValidator<PayWithWalletCommand> validator,
        IFinanceUnitOfWork unitOfWork,
        IMediator mediator)
    {
        _billRepository = billRepository ?? throw new ArgumentNullException(nameof(billRepository));
        _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
        _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<ApplicationResult<PayWithWalletResponse>> Handle(
        PayWithWalletCommand request,
        CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginAsync(cancellationToken);
        try
        {
            // Validate request
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
            {
                var errors = validation.Errors.Select(e => e.ErrorMessage).ToList();
                return ApplicationResult<PayWithWalletResponse>.Failure(errors, "اعتبارسنجی ناموفق");
            }

            // Step 1: Check Bills and Payments
            var payment = await _paymentRepository.GetByIdAsync(request.PaymentId, cancellationToken);
            if (payment == null)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<PayWithWalletResponse>.Failure("پرداخت یافت نشد");
            }

            var bill = await _billRepository.GetWithAllDataAsync(request.BillId, cancellationToken);
            if (bill == null)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<PayWithWalletResponse>.Failure("صورت حساب یافت نشد");
            }

            // Verify payment belongs to bill
            if (payment.BillId != bill.Id)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<PayWithWalletResponse>.Failure("پرداخت متعلق به این صورت حساب نیست");
            }

            // Check if payment is in pending status
            if (payment.Status != PaymentStatus.Pending)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<PayWithWalletResponse>.Failure($"پرداخت در وضعیت قابل پردازش نیست. وضعیت فعلی: {GetPaymentStatusText(payment.Status)}");
            }

            // Check if bill is payable
            if (bill.Status == BillStatus.FullyPaid || 
                bill.Status == BillStatus.Cancelled || 
                bill.Status == BillStatus.Voided ||
                bill.Status == BillStatus.WrittenOff ||
                bill.Status == BillStatus.Credited ||
                bill.Status == BillStatus.Disputed)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<PayWithWalletResponse>.Failure($"صورت حساب قابل پرداخت نیست. وضعیت فعلی: {GetBillStatusText(bill.Status)}");
            }

            // Calculate payment amount
            var paymentAmount = request.AmountRials ?? payment.Amount.AmountRials;
            if (paymentAmount <= 0)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<PayWithWalletResponse>.Failure("مبلغ پرداخت باید بیشتر از صفر باشد");
            }

            if (paymentAmount > bill.RemainingAmount.AmountRials)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<PayWithWalletResponse>.Failure($"مبلغ پرداخت نمی‌تواند بیشتر از مبلغ باقی‌مانده صورت حساب باشد. مبلغ باقی‌مانده: {bill.RemainingAmount.AmountRials:N0} ریال");
            }

            // Step 2: Get Wallet and Check Balance
            var wallet = await _walletRepository.GetByIdWithRefreshedBalanceAsync(request.WalletId, cancellationToken);
            if (wallet == null)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<PayWithWalletResponse>.Failure("کیف پول یافت نشد");
            }

            // Check if wallet is active
            if (wallet.Status != Domain.Enums.WalletStatus.Active)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<PayWithWalletResponse>.Failure($"کیف پول فعال نیست. وضعیت فعلی: {wallet.Status}");
            }

            // Check wallet balance
            var walletBalance = wallet.Balance.AmountRials;
            if (walletBalance < paymentAmount)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<PayWithWalletResponse>.Failure(
                    $"موجودی کیف پول کافی نیست. موجودی فعلی: {walletBalance:N0} ریال، مبلغ مورد نیاز: {paymentAmount:N0} ریال");
            }

            // Step 3: Create Wallet Transaction for Payment
            var walletTransaction = wallet.PayBill(
                amount: Money.FromRials(paymentAmount),
                billId: bill.Id,
                billNumber: bill.BillNumber,
                referenceId: payment.Id.ToString(),
                description: request.Description ?? $"پرداخت صورت حساب {bill.BillNumber} با کیف پول",
                externalReference: request.ExternalReference ?? $"PAYMENT_{bill.BillNumber}"
            );

            // Add additional metadata to the transaction
            walletTransaction.AddMetadata("WalletId", wallet.Id.ToString());
            walletTransaction.AddMetadata("PaymentMethod", "Wallet");
            walletTransaction.AddMetadata("PaymentAmount", paymentAmount.ToString());
            walletTransaction.AddMetadata("ProcessedAt", DateTime.UtcNow.ToString("O"));

            // Step 4: Save Wallet Updates
            await _walletRepository.UpdateAsync(wallet, cancellationToken: cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Step 5: Send to CompletePaymentCommand to Handle Confirm Payment
            var completePaymentCommand = new CompletePaymentCommand
            {
                PaymentId = payment.Id,
                GatewayTransactionId = $"WALLET_{payment.Id}",
                GatewayReference = request.ExternalReference ?? $"WALLET_PAY_{bill.BillNumber}"
            };

            var completeResult = await _mediator.Send(completePaymentCommand, cancellationToken);
            if (!completeResult.IsSuccess)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<PayWithWalletResponse>.Failure(
                    completeResult.Errors, "خطا در تکمیل پرداخت");
            }

            // Commit all changes
            await _unitOfWork.CommitAsync(cancellationToken);

            // Get updated payment and wallet for response
            var updatedPayment = await _paymentRepository.GetByIdAsync(payment.Id, cancellationToken);
            var updatedWallet = await _walletRepository.GetByIdWithRefreshedBalanceAsync(request.WalletId, cancellationToken);

            // Prepare response
            var response = new PayWithWalletResponse
            {
                PaymentId = updatedPayment!.Id,
                BillId = bill.Id,
                BillNumber = bill.BillNumber,
                AmountPaidRials = paymentAmount,
                PaymentStatus = updatedPayment.Status.ToString(),
                PaymentStatusText = GetPaymentStatusText(updatedPayment.Status),
                BillStatus = bill.Status.ToString(),
                BillStatusText = GetBillStatusText(bill.Status),
                BillRemainingAmountRials = bill.RemainingAmount.AmountRials,
                WalletBalanceAfterPaymentRials = updatedWallet?.Balance.AmountRials ?? 0,
                ProcessedAt = updatedPayment.CompletedAt ?? DateTime.UtcNow,
                Description = request.Description,
                ExternalReference = request.ExternalReference,
                WalletTransactionId = walletTransaction.Id,
                Metadata = request.Metadata ?? new Dictionary<string, string>()
            };

            return ApplicationResult<PayWithWalletResponse>.Success(response, "پرداخت با کیف پول با موفقیت انجام شد");
        }
        catch (DbUpdateConcurrencyException ex)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return ApplicationResult<PayWithWalletResponse>.Failure(
                ex, "خطا در پرداخت با کیف پول - تداخل در داده‌ها. لطفاً مجدداً تلاش کنید");
        }
        catch (DbUpdateException ex)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return ApplicationResult<PayWithWalletResponse>.Failure(
                ex, "خطا در ذخیره اطلاعات پرداخت با کیف پول");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return ApplicationResult<PayWithWalletResponse>.Failure(
                ex, "خطا در پرداخت با کیف پول");
        }
    }

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

    private static string GetPaymentStatusText(PaymentStatus status)
    {
        return status switch
        {
            PaymentStatus.Pending => "در انتظار",
            PaymentStatus.Processing => "در حال پردازش",
            PaymentStatus.Completed => "تکمیل شده",
            PaymentStatus.Failed => "ناموفق",
            PaymentStatus.Cancelled => "لغو شده",
            PaymentStatus.Refunded => "برگشت داده شده",
            _ => status.ToString()
        };
    }
}
