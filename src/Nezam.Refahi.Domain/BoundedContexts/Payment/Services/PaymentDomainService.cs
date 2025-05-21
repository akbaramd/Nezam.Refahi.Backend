using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nezam.Refahi.Domain.BoundedContexts.Accommodation.ValueObjects;
using Nezam.Refahi.Domain.BoundedContexts.Payment.Aggregates;
using Nezam.Refahi.Domain.BoundedContexts.Payment.Repositories;
using Nezam.Refahi.Domain.BoundedContexts.Payment.ValueObjects;
using Nezam.Refahi.Domain.BoundedContexts.Shared;

namespace Nezam.Refahi.Domain.BoundedContexts.Payment.Services
{
    /// <summary>
    /// Domain service for handling payment-related business operations.
    /// Following DDD principles, this service coordinates operations across different entities
    /// but contains no infrastructure concerns.
    /// </summary>
    public class PaymentDomainService
    {
        private readonly IPaymentRepository _paymentRepository;
        
        public PaymentDomainService(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
        }

        /// <summary>
        /// Initiates a new payment transaction for a reservation.
        /// </summary>
        /// <param name="reservationId">The ID of the reservation</param>
        /// <param name="customerId">The ID of the customer making the payment</param>
        /// <param name="amount">The payment amount</param>
        /// <param name="paymentMethod">The method of payment</param>
        /// <param name="gateway">The payment gateway used</param>
        /// <returns>The created payment transaction</returns>
        public async Task<PaymentTransaction> InitiatePaymentAsync(
            Guid reservationId,
            Guid? customerId,
            Money amount,
            PaymentMethod paymentMethod,
            string? gateway)
        {
            if (amount == null)
                throw new ArgumentNullException(nameof(amount), "Payment amount cannot be null");
            
            if (amount.Amount <= 0)
                throw new ArgumentException("Payment amount must be greater than zero", nameof(amount));
            
            var transaction = new PaymentTransaction(
              Guid.NewGuid(),reservationId,
                customerId,
                amount,
                paymentMethod,
                gateway);
            
            await _paymentRepository.AddAsync(transaction);
            
            return transaction;
        }
        
        /// <summary>
        /// Marks a payment transaction as completed.
        /// </summary>
        /// <param name="transactionId">The ID of the transaction to complete</param>
        /// <param name="transactionReference">The reference number from the payment provider</param>
        /// <param name="receiptNumber">Optional receipt number</param>
        /// <returns>True if the transaction was completed successfully</returns>
        public async Task<bool> CompletePaymentAsync(
            Guid transactionId,
            string transactionReference,
            string? receiptNumber = null)
        {
            var transaction = await _paymentRepository.GetByIdAsync(transactionId);
            
            if (transaction == null)
                throw new InvalidOperationException($"Payment transaction with ID {transactionId} not found");
            
            if (transaction.Status == PaymentStatus.Completed)
                return true; // Already completed
            
            transaction.SetTransactionReference(transactionReference);
            transaction.MarkAsCompleted(receiptNumber);
            
            await _paymentRepository.UpdateAsync(transaction);
            
            return true;
        }
        
        /// <summary>
        /// Records a failed payment attempt.
        /// </summary>
        /// <param name="transactionId">The ID of the transaction that failed</param>
        /// <param name="failureReason">The reason for the failure</param>
        /// <returns>True if the transaction was marked as failed</returns>
        public async Task<bool> RecordFailedPaymentAsync(
            Guid transactionId,
            string failureReason)
        {
            if (string.IsNullOrWhiteSpace(failureReason))
                throw new ArgumentException("Failure reason cannot be empty", nameof(failureReason));
            
            var transaction = await _paymentRepository.GetByIdAsync(transactionId);
            
            if (transaction == null)
                throw new InvalidOperationException($"Payment transaction with ID {transactionId} not found");
            
            transaction.MarkAsFailed(failureReason);
            
            await _paymentRepository.UpdateAsync(transaction);
            
            return true;
        }
        
        /// <summary>
        /// Processes a refund for a completed payment.
        /// </summary>
        /// <param name="originalTransactionId">The ID of the original transaction to refund</param>
        /// <param name="refundAmount">The amount to refund (defaults to full amount if null)</param>
        /// <param name="gateway">The payment gateway used for the refund</param>
        /// <returns>The refund transaction</returns>
        public async Task<PaymentTransaction> ProcessRefundAsync(
            Guid originalTransactionId,
            Money? refundAmount = null,
            string? gateway = null)
        {
            var originalTransaction = await _paymentRepository.GetByIdAsync(originalTransactionId);
            
            if (originalTransaction == null)
                throw new InvalidOperationException($"Original payment transaction with ID {originalTransactionId} not found");
            
            if (!originalTransaction.CanRefund())
                throw new InvalidOperationException("The original transaction cannot be refunded");
            
            var actualRefundAmount = refundAmount ?? originalTransaction.Amount;
            
            if (actualRefundAmount.Amount > originalTransaction.Amount.Amount)
                throw new ArgumentException("Refund amount cannot exceed the original payment amount", nameof(refundAmount));
            
            var refundTransaction = new PaymentTransaction(
              Guid.NewGuid(),
                originalTransaction.ReservationId,
                originalTransaction.CustomerId,
                actualRefundAmount,
                originalTransaction.PaymentMethod,
                gateway ?? originalTransaction.Gateway,
                isRefund: true,
                refundedTransactionId: originalTransactionId);
            
            await _paymentRepository.AddAsync(refundTransaction);
            
            return refundTransaction;
        }
        
        /// <summary>
        /// Gets the payment history for a reservation.
        /// </summary>
        /// <param name="reservationId">The ID of the reservation</param>
        /// <returns>A collection of payment transactions for the reservation</returns>
        public async Task<IEnumerable<PaymentTransaction>> GetPaymentHistoryForReservationAsync(Guid reservationId)
        {
            return await _paymentRepository.GetByReservationIdAsync(reservationId);
        }
        
        /// <summary>
        /// Calculates the total amount paid for a reservation.
        /// </summary>
        /// <param name="reservationId">The ID of the reservation</param>
        /// <returns>The total amount paid (completed payments minus refunds)</returns>
        public async Task<decimal> CalculateTotalPaidAmountAsync(Guid reservationId)
        {
            var transactions = await _paymentRepository.GetByReservationIdAsync(reservationId);
            
            var completedTransactions = transactions.Where(t => t.Status == PaymentStatus.Completed);
            
            decimal totalPaid = completedTransactions
                .Where(t => !t.IsRefund)
                .Sum(t => t.Amount.Amount);
            
            decimal totalRefunded = completedTransactions
                .Where(t => t.IsRefund)
                .Sum(t => t.Amount.Amount);
            
            return totalPaid - totalRefunded;
        }
        
        /// <summary>
        /// Checks if a reservation is fully paid.
        /// </summary>
        /// <param name="reservationId">The ID of the reservation</param>
        /// <param name="totalAmount">The total amount due for the reservation</param>
        /// <returns>True if the reservation is fully paid</returns>
        public async Task<bool> IsReservationFullyPaidAsync(Guid reservationId, decimal totalAmount)
        {
            if (totalAmount <= 0)
                return true;
            
            var amountPaid = await CalculateTotalPaidAmountAsync(reservationId);
            
            // Allow for small rounding differences (e.g., currency conversion)
            const decimal tolerance = 0.01m;
            return amountPaid >= (totalAmount - tolerance);
        }

        #region Authorization and Capture Flow

        /// <summary>
        /// Authorizes a payment without capturing funds. This places a hold on the funds.
        /// </summary>
        public async Task<PaymentTransaction> AuthorizePaymentAsync(
            Guid reservationId,
            Guid? customerId,
            Money amount,
            PaymentMethod paymentMethod,
            string? gateway,
            string authorizationCode)
        {
            if (string.IsNullOrWhiteSpace(authorizationCode))
                throw new ArgumentException("Authorization code cannot be empty", nameof(authorizationCode));
                
            var transaction = new PaymentTransaction(
                Guid.NewGuid(),
                reservationId,
                customerId,
                amount,
                paymentMethod,
                gateway);
                
            transaction.MarkAsAuthorized(authorizationCode);
            
            await _paymentRepository.AddAsync(transaction);
            
            return transaction;
        }
        
        /// <summary>
        /// Captures an authorized payment, either fully or partially.
        /// </summary>
        public async Task<PaymentTransaction> CaptureAuthorizedPaymentAsync(
            Guid transactionId,
            Money? captureAmount = null,
            string? captureReference = null)
        {
            var transaction = await _paymentRepository.GetByIdAsync(transactionId);
            
            if (transaction == null)
                throw new InvalidOperationException($"Payment transaction with ID {transactionId} not found");
                
            if (transaction.Status != PaymentStatus.Authorized)
                throw new InvalidOperationException("Only authorized payments can be captured");
                
            var amountToCapture = captureAmount ?? transaction.AuthorizedAmount;
            
            transaction.MarkAsCaptured(amountToCapture!, captureReference);
            await _paymentRepository.UpdateAsync(transaction);
            
            return transaction;
        }
        
        /// <summary>
        /// Cancels an authorized payment that hasn't been captured yet.
        /// </summary>
        public async Task<bool> CancelAuthorizationAsync(Guid transactionId, string reason)
        {
            var transaction = await _paymentRepository.GetByIdAsync(transactionId);
            
            if (transaction == null)
                throw new InvalidOperationException($"Payment transaction with ID {transactionId} not found");
                
            if (transaction.Status != PaymentStatus.Authorized)
                throw new InvalidOperationException("Only authorized payments can be canceled");
                
            transaction.MarkAsFailed($"Authorization canceled: {reason}");
            await _paymentRepository.UpdateAsync(transaction);
            
            return true;
        }
        
        #endregion
        
        #region Advanced Refund Operations
        
        /// <summary>
        /// Processes a refund with an optional fee deduction and provides a reason.
        /// </summary>
        public async Task<PaymentTransaction> ProcessRefundWithFeeAsync(
            Guid originalTransactionId,
            Money refundAmount,
            Money? refundFee = null,
            string? reason = null,
            string? gateway = null)
        {
            var originalTransaction = await _paymentRepository.GetByIdAsync(originalTransactionId);
            
            if (originalTransaction == null)
                throw new InvalidOperationException($"Original payment transaction with ID {originalTransactionId} not found");
                
            if (!originalTransaction.CanRefund())
                throw new InvalidOperationException("The original transaction cannot be refunded");
                
            if (refundAmount.Amount > originalTransaction.Amount.Amount)
                throw new ArgumentException("Refund amount cannot exceed original payment amount", nameof(refundAmount));
                
            // Process the basic refund first
            var refundTransaction = new PaymentTransaction(
                Guid.NewGuid(),
                originalTransaction.ReservationId,
                originalTransaction.CustomerId,
                refundAmount,
                originalTransaction.PaymentMethod,
                gateway ?? originalTransaction.Gateway,
                isRefund: true,
                refundedTransactionId: originalTransactionId);
                
            // Set the refund reason if provided
            if (!string.IsNullOrWhiteSpace(reason))
            {
                refundTransaction.SetRefundReason(reason);
            }
            
            // Apply the refund fee if provided
            if (refundFee != null && refundFee.Amount > 0)
            {
                refundTransaction.ApplyRefundFee(refundFee);
            }
            
            await _paymentRepository.AddAsync(refundTransaction);
            
            return refundTransaction;
        }
        
        /// <summary>
        /// Calculates the maximum refundable amount for a transaction.
        /// Takes into account any partial refunds already processed.
        /// </summary>
        public async Task<Money> CalculateMaxRefundableAmountAsync(Guid transactionId)
        {
            var transaction = await _paymentRepository.GetByIdAsync(transactionId);
            
            if (transaction == null)
                throw new InvalidOperationException($"Payment transaction with ID {transactionId} not found");
                
            if (!transaction.CanRefund())
                throw new InvalidOperationException("This transaction cannot be refunded");
                
            // Get all refunds already processed for this transaction
            var existingRefunds = await _paymentRepository.GetByReservationIdAsync(transaction.ReservationId);
            var relatedRefunds = existingRefunds.Where(t => 
                t.IsRefund && 
                t.RefundedTransactionId == transactionId && 
                t.Status == PaymentStatus.Completed);
                
            // Calculate total already refunded
            decimal alreadyRefunded = relatedRefunds.Sum(r => r.Amount.Amount);
            
            // Calculate remaining refundable amount
            decimal maxRefundable = transaction.Amount.Amount - alreadyRefunded;
            
            if (maxRefundable <= 0)
                throw new InvalidOperationException("No refundable amount remains");
                
            return new Money(maxRefundable, transaction.Amount.Currency);
        }
        
        #endregion
        
        #region Installment Plans
        
        /// <summary>
        /// Creates a payment installment plan for a reservation.
        /// </summary>
        public Task<PaymentInstallmentPlan> CreateInstallmentPlanAsync(
            Guid reservationId, 
            Money totalAmount, 
            int numberOfInstallments, 
            int daysBetweenPayments,
            PaymentMethod paymentMethod)
        {
            if (numberOfInstallments <= 0)
                throw new ArgumentException("Number of installments must be positive", nameof(numberOfInstallments));
                
            if (daysBetweenPayments < 0)
                throw new ArgumentException("Days between payments cannot be negative", nameof(daysBetweenPayments));
                
            // Create the installment plan
            var plan = new PaymentInstallmentPlan(
                totalAmount,
                numberOfInstallments,
                DateTimeOffset.UtcNow,
                daysBetweenPayments);
                
            // Here you would typically store this in a repository
            // But for this example, we'll just return it
            
            return Task.FromResult(plan);
        }
        
        /// <summary>
        /// Processes a payment for a specific installment.
        /// </summary>
        public async Task<PaymentTransaction> ProcessInstallmentPaymentAsync(
            Guid installmentPlanId,
            int installmentNumber,
            int totalInstallments,
            Guid reservationId,
            Guid? customerId,
            Money amount,
            PaymentMethod paymentMethod,
            string? gateway,
            string transactionReference)
        {
            if (installmentNumber <= 0 || installmentNumber > totalInstallments)
                throw new ArgumentException("Invalid installment number", nameof(installmentNumber));
                
            // Create the payment transaction
            var transaction = new PaymentTransaction(
                Guid.NewGuid(),
                reservationId,
                customerId,
                amount,
                paymentMethod,
                gateway);
                
            // Link to the installment plan
            transaction.LinkToInstallmentPlan(installmentPlanId, installmentNumber, totalInstallments);
            
            // Complete the payment
            transaction.SetTransactionReference(transactionReference);
            transaction.MarkAsCompleted();
            
            await _paymentRepository.AddAsync(transaction);
            
            return transaction;
        }
        
        #endregion
        
        #region Multi-Currency Support
        
        /// <summary>
        /// Processes a payment in a foreign currency with conversion to local currency.
        /// </summary>
        public async Task<PaymentTransaction> ProcessForeignCurrencyPaymentAsync(
            Guid reservationId,
            Guid? customerId,
            Money foreignAmount,
            Money localAmount,
            decimal exchangeRate,
            PaymentMethod paymentMethod,
            string? gateway)
        {
            // Validate the exchange rate calculation
            var calculatedLocalAmount = Math.Round(foreignAmount.Amount * exchangeRate, 2);
            var allowedDifference = Math.Round(localAmount.Amount * 0.02m, 2); // 2% tolerance
            
            if (Math.Abs(calculatedLocalAmount - localAmount.Amount) > allowedDifference)
                throw new ArgumentException("Exchange rate calculation does not match provided local amount");
                
            // Create the payment transaction with the original foreign amount
            var transaction = new PaymentTransaction(
                Guid.NewGuid(),
                reservationId,
                customerId,
                foreignAmount,
                paymentMethod,
                gateway);
                
            // Record the currency exchange information
            transaction.RecordCurrencyExchange(localAmount, exchangeRate);
            
            await _paymentRepository.AddAsync(transaction);
            
            return transaction;
        }
        
        #endregion
        
        #region Dispute Handling
        
        /// <summary>
        /// Records a dispute against a completed payment.
        /// </summary>
        public async Task<PaymentTransaction> RecordDisputeAsync(
            Guid transactionId, 
            string disputeReason)
        {
            var transaction = await _paymentRepository.GetByIdAsync(transactionId);
            
            if (transaction == null)
                throw new InvalidOperationException($"Payment transaction with ID {transactionId} not found");
                
            if (transaction.Status != PaymentStatus.Completed && transaction.Status != PaymentStatus.PartiallyCompleted)
                throw new InvalidOperationException("Only completed payments can be disputed");
                
            if (string.IsNullOrWhiteSpace(disputeReason))
                throw new ArgumentException("Dispute reason cannot be empty", nameof(disputeReason));
                
            transaction.MarkAsDisputed(disputeReason);
            await _paymentRepository.UpdateAsync(transaction);
            
            return transaction;
        }
        
        /// <summary>
        /// Resolves a payment dispute with the specified outcome.
        /// </summary>
        public async Task<PaymentTransaction> ResolveDisputeAsync(
            Guid transactionId, 
            bool inMerchantFavor, 
            string resolutionDetails)
        {
            var transaction = await _paymentRepository.GetByIdAsync(transactionId);
            
            if (transaction == null)
                throw new InvalidOperationException($"Payment transaction with ID {transactionId} not found");
                
            if (transaction.Status != PaymentStatus.Disputed)
                throw new InvalidOperationException("Only disputed payments can be resolved");
                
            if (string.IsNullOrWhiteSpace(resolutionDetails))
                throw new ArgumentException("Resolution details cannot be empty", nameof(resolutionDetails));
                
            transaction.ResolveDispute(inMerchantFavor, resolutionDetails);
            await _paymentRepository.UpdateAsync(transaction);
            
            return transaction;
        }
        
        #endregion
        
        #region Fraud Handling
        
        /// <summary>
        /// Marks a payment as requiring additional verification for fraud prevention.
        /// </summary>
        public async Task<PaymentTransaction> MarkPaymentForVerificationAsync(
            Guid transactionId, 
            string verificationReason)
        {
            var transaction = await _paymentRepository.GetByIdAsync(transactionId);
            
            if (transaction == null)
                throw new InvalidOperationException($"Payment transaction with ID {transactionId} not found");
                
            if (transaction.Status != PaymentStatus.Pending && transaction.Status != PaymentStatus.Authorized)
                throw new InvalidOperationException("Only pending or authorized payments can be marked for verification");
                
            if (string.IsNullOrWhiteSpace(verificationReason))
                throw new ArgumentException("Verification reason cannot be empty", nameof(verificationReason));
                
            transaction.MarkAsVerifying(verificationReason);
            await _paymentRepository.UpdateAsync(transaction);
            
            return transaction;
        }
        
        /// <summary>
        /// Marks a payment as rejected due to fraud detection.
        /// </summary>
        public async Task<PaymentTransaction> RejectPaymentAsFraudulentAsync(
            Guid transactionId, 
            string fraudReason)
        {
            var transaction = await _paymentRepository.GetByIdAsync(transactionId);
            
            if (transaction == null)
                throw new InvalidOperationException($"Payment transaction with ID {transactionId} not found");
                
            if (transaction.Status == PaymentStatus.Completed)
                throw new InvalidOperationException("Completed payments cannot be marked as fraudulent");
                
            if (string.IsNullOrWhiteSpace(fraudReason))
                throw new ArgumentException("Fraud reason cannot be empty", nameof(fraudReason));
                
            transaction.MarkAsFraudRejected(fraudReason);
            await _paymentRepository.UpdateAsync(transaction);
            
            return transaction;
        }
        
        #endregion
        
        #region Payment Retry
        
        /// <summary>
        /// Creates a new payment attempt based on a failed previous attempt.
        /// </summary>
        public async Task<PaymentTransaction> RetryFailedPaymentAsync(
            Guid failedTransactionId,
            PaymentMethod? newPaymentMethod = null,
            string? newGateway = null)
        {
            var failedTransaction = await _paymentRepository.GetByIdAsync(failedTransactionId);
            
            if (failedTransaction == null)
                throw new InvalidOperationException($"Failed payment transaction with ID {failedTransactionId} not found");
                
            if (failedTransaction.Status != PaymentStatus.Failed && 
                failedTransaction.Status != PaymentStatus.FraudRejected &&
                failedTransaction.Status != PaymentStatus.AuthorizationExpired)
            {
                throw new InvalidOperationException("Only failed payments can be retried");
            }
            
            // Create a new transaction with the same details
            var newTransaction = new PaymentTransaction(
                Guid.NewGuid(),
                failedTransaction.ReservationId,
                failedTransaction.CustomerId,
                failedTransaction.Amount,
                newPaymentMethod ?? failedTransaction.PaymentMethod,
                newGateway ?? failedTransaction.Gateway);
                
            // Link to the previous attempt
            newTransaction.LinkToPreviousAttempt(failedTransactionId, failedTransaction.AttemptNumber + 1);
            
            await _paymentRepository.AddAsync(newTransaction);
            
            return newTransaction;
        }
        
        #endregion
    }
}
