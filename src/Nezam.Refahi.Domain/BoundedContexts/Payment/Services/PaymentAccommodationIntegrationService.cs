using System;
using System.Threading.Tasks;
using Nezam.Refahi.Domain.BoundedContexts.Accommodation.Aggregates;
using Nezam.Refahi.Domain.BoundedContexts.Accommodation.Repositories;
using Nezam.Refahi.Domain.BoundedContexts.Accommodation.ValueObjects;
using Nezam.Refahi.Domain.BoundedContexts.Payment.Aggregates;
using Nezam.Refahi.Domain.BoundedContexts.Payment.Repositories;
using Nezam.Refahi.Domain.BoundedContexts.Payment.ValueObjects;
using Nezam.Refahi.Domain.BoundedContexts.Shared;

namespace Nezam.Refahi.Domain.BoundedContexts.Payment.Services
{
    /// <summary>
    /// Acts as an Anti-Corruption Layer between the Payment and Accommodation bounded contexts.
    /// Handles coordination between the two contexts while preserving their domain models.
    /// </summary>
    public class PaymentAccommodationIntegrationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly PaymentDomainService _paymentDomainService;
        
        public PaymentAccommodationIntegrationService(
            IReservationRepository reservationRepository,
            IPaymentRepository paymentRepository,
            PaymentDomainService paymentDomainService)
        {
            _reservationRepository = reservationRepository ?? throw new ArgumentNullException(nameof(reservationRepository));
            _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
            _paymentDomainService = paymentDomainService ?? throw new ArgumentNullException(nameof(paymentDomainService));
        }
        
        #region Standard Payment Processing

        /// <summary>
        /// Processes a payment for a reservation.
        /// </summary>
        /// <param name="reservationId">The ID of the reservation</param>
        /// <param name="customerId">The ID of the customer</param>
        /// <param name="amount">The payment amount</param>
        /// <param name="paymentMethod">The method of payment</param>
        /// <param name="gateway">The payment gateway used</param>
        /// <returns>The created payment transaction</returns>
        public async Task<PaymentTransaction> ProcessReservationPaymentAsync(
            Guid reservationId,
            Guid? customerId,
            Money amount,
            PaymentMethod paymentMethod,
            string? gateway)
        {
            // Validate that the reservation exists
            var reservation = await _reservationRepository.GetByIdAsync(reservationId);
            
            if (reservation == null)
                throw new InvalidOperationException($"Reservation with ID {reservationId} not found");
            
            // Create a payment transaction
            var transaction = await _paymentDomainService.InitiatePaymentAsync(
                reservationId,
                customerId,
                amount,
                paymentMethod,
                gateway);
            
            return transaction;
        }
        
        /// <summary>
        /// Processes a refund for a reservation payment.
        /// </summary>
        /// <param name="transactionId">The ID of the payment transaction to refund</param>
        /// <param name="refundAmount">The amount to refund (defaults to full amount if null)</param>
        /// <param name="gateway">The payment gateway to use</param>
        /// <returns>The refund transaction</returns>
        public async Task<PaymentTransaction> ProcessRefundAsync(
            Guid transactionId,
            Money? refundAmount = null,
            string? gateway = null)
        {
            var transaction = await _paymentRepository.GetByIdAsync(transactionId);
            
            if (transaction == null)
                throw new InvalidOperationException($"Payment transaction with ID {transactionId} not found");
            
            var refundTransaction = await _paymentDomainService.ProcessRefundAsync(
                transactionId,
                refundAmount,
                gateway);
            
            // Update the reservation
            var reservation = await _reservationRepository.GetByIdAsync(transaction.ReservationId);
            
            if (reservation != null)
            {
                // Record the refund in the Accommodation context
                // Using the actual refundAmount or the transaction amount if null
                var actualRefundAmount = refundAmount ?? refundTransaction.Amount;
                reservation.RecordRefund(refundTransaction.Id, actualRefundAmount);
                
                await _reservationRepository.UpdateAsync(reservation);
            }
            
            return refundTransaction;
        }
        
        /// <summary>
        /// Checks if a reservation is fully paid.
        /// </summary>
        /// <param name="reservationId">The ID of the reservation</param>
        /// <returns>True if the reservation is fully paid</returns>
        public async Task<bool> IsReservationFullyPaidAsync(Guid reservationId)
        {
            var reservation = await _reservationRepository.GetByIdAsync(reservationId);
            
            if (reservation == null)
                throw new InvalidOperationException($"Reservation with ID {reservationId} not found");
            
            return await _paymentDomainService.IsReservationFullyPaidAsync(
                reservationId,
                reservation.TotalPrice.Amount);
        }
        
        /// <summary>
        /// Handles a completed payment by updating the reservation status.
        /// </summary>
        /// <param name="transactionId">The ID of the completed transaction</param>
        /// <returns>True if the reservation was updated successfully</returns>
        public async Task<bool> HandlePaymentCompletedAsync(Guid transactionId)
        {
            var transaction = await _paymentRepository.GetByIdAsync(transactionId);
            
            if (transaction == null)
                throw new InvalidOperationException($"Payment transaction with ID {transactionId} not found");
            
            if (transaction.Status != PaymentStatus.Completed)
                throw new InvalidOperationException("Transaction is not in Completed status");
            
            var reservation = await _reservationRepository.GetByIdAsync(transaction.ReservationId);
            
            if (reservation == null)
                throw new InvalidOperationException($"Reservation with ID {transaction.ReservationId} not found");
            
            // Record the payment in the Accommodation context using ProcessPartialPayment
            reservation.ProcessPartialPayment(
                transaction.Id,
                transaction.Amount);
            
            // The ProcessPartialPayment method will handle updating the status
            // and will confirm the reservation if it's fully paid
            
            await _reservationRepository.UpdateAsync(reservation);
            
            return true;
        }
        
        /// <summary>
        /// Handles a failed payment by updating the reservation status.
        /// </summary>
        /// <param name="transactionId">The ID of the failed transaction</param>
        /// <returns>True if the reservation was updated successfully</returns>
        public async Task<bool> HandlePaymentFailedAsync(Guid transactionId)
        {
            var transaction = await _paymentRepository.GetByIdAsync(transactionId);
            
            if (transaction == null)
                throw new InvalidOperationException($"Payment transaction with ID {transactionId} not found");
            
            if (transaction.Status != PaymentStatus.Failed)
                throw new InvalidOperationException("Transaction is not in Failed status");
            
            var reservation = await _reservationRepository.GetByIdAsync(transaction.ReservationId);
            
            if (reservation == null)
                throw new InvalidOperationException($"Reservation with ID {transaction.ReservationId} not found");
            
            // Record the failed payment in the Accommodation context
            reservation.MarkPaymentAsFailed(
                transaction.Id,
                transaction.FailureReason ?? "Unknown failure reason");
            
            await _reservationRepository.UpdateAsync(reservation);
            
            return true;
        }

        #endregion

        #region Authorization and Capture Flow

        /// <summary>
        /// Pre-authorizes a payment for a reservation without capturing funds.
        /// Useful for holding funds temporarily until reservation is confirmed.
        /// </summary>
        public async Task<PaymentTransaction> AuthorizeReservationPaymentAsync(
            Guid reservationId,
            Guid? customerId,
            Money amount,
            PaymentMethod paymentMethod,
            string? gateway,
            string authorizationCode)
        {
            // Validate that the reservation exists
            var reservation = await _reservationRepository.GetByIdAsync(reservationId);
            
            if (reservation == null)
                throw new InvalidOperationException($"Reservation with ID {reservationId} not found");
            
            // Authorize the payment
            var transaction = await _paymentDomainService.AuthorizePaymentAsync(
                reservationId,
                customerId,
                amount,
                paymentMethod,
                gateway,
                authorizationCode);
            
            // Update the reservation to indicate an authorization is pending
            // We'll use MarkPaymentPending since there's no direct SetAuthorizationPending method
            reservation.MarkPaymentPending();
                
            await _reservationRepository.UpdateAsync(reservation);
            
            return transaction;
        }
        
        /// <summary>
        /// Captures a previously authorized payment for a confirmed reservation.
        /// </summary>
        public async Task<PaymentTransaction> CaptureAuthorizedPaymentAsync(
            Guid transactionId,
            Money? captureAmount = null,
            string? captureReference = null)
        {
            var transaction = await _paymentRepository.GetByIdAsync(transactionId);
            
            if (transaction == null)
                throw new InvalidOperationException($"Payment transaction with ID {transactionId} not found");
            
            // Capture the payment
            var capturedTransaction = await _paymentDomainService.CaptureAuthorizedPaymentAsync(
                transactionId,
                captureAmount,
                captureReference);
            
            // Update the reservation
            var reservation = await _reservationRepository.GetByIdAsync(transaction.ReservationId);
            
            if (reservation == null)
                throw new InvalidOperationException($"Reservation with ID {transaction.ReservationId} not found");
            
            // Record the actual payment in the Accommodation context
            var actualAmount = captureAmount ?? transaction.AuthorizedAmount ?? transaction.Amount;
            reservation.ProcessPartialPayment(
                transaction.Id,
                actualAmount);
            
            await _reservationRepository.UpdateAsync(reservation);
            
            return capturedTransaction;
        }
        
        /// <summary>
        /// Cancels a previously authorized payment that hasn't been captured.
        /// </summary>
        public async Task<bool> CancelAuthorizationAsync(Guid transactionId, string reason)
        {
            var transaction = await _paymentRepository.GetByIdAsync(transactionId);
            
            if (transaction == null)
                throw new InvalidOperationException($"Payment transaction with ID {transactionId} not found");
            
            // Cancel the authorization
            await _paymentDomainService.CancelAuthorizationAsync(transactionId, reason);
            
            // Update the reservation
            var reservation = await _reservationRepository.GetByIdAsync(transaction.ReservationId);
            
            if (reservation == null)
                throw new InvalidOperationException($"Reservation with ID {transaction.ReservationId} not found");
            
            // We'll use CancelWithReason since there's no direct CancelAuthorizationPending method
            reservation.CancelWithReason($"Authorization canceled: {reason}");
                
            await _reservationRepository.UpdateAsync(reservation);
            
            return true;
        }
        
        #endregion
        
        #region Installment Payments
        
        /// <summary>
        /// Creates an installment payment plan for a reservation.
        /// </summary>
        public async Task<PaymentInstallmentPlan> CreateInstallmentPlanAsync(
            Guid reservationId,
            int numberOfInstallments,
            int daysBetweenPayments)
        {
            // Validate that the reservation exists
            var reservation = await _reservationRepository.GetByIdAsync(reservationId);
            
            if (reservation == null)
                throw new InvalidOperationException($"Reservation with ID {reservationId} not found");
            
            // Calculate the total amount from the reservation
            var totalAmount = new Money(reservation.TotalPrice.Amount, reservation.TotalPrice.Currency);
            
            // Create the installment plan
            var plan = await _paymentDomainService.CreateInstallmentPlanAsync(
                reservationId,
                totalAmount,
                numberOfInstallments,
                daysBetweenPayments,
                PaymentMethod.CreditCard); // Default to credit card
            
            // The Reservation class doesn't have a specific SetPaymentPlan method
            // so we'll just mark payment as pending
            reservation.MarkPaymentPending();
                
            await _reservationRepository.UpdateAsync(reservation);
            
            return plan;
        }
        
        /// <summary>
        /// Processes a payment for a specific installment in a payment plan.
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
            // Validate that the reservation exists
            var reservation = await _reservationRepository.GetByIdAsync(reservationId);
            
            if (reservation == null)
                throw new InvalidOperationException($"Reservation with ID {reservationId} not found");
            
            // Process the installment payment
            var transaction = await _paymentDomainService.ProcessInstallmentPaymentAsync(
                installmentPlanId,
                installmentNumber,
                totalInstallments,
                reservationId,
                customerId,
                amount,
                paymentMethod,
                gateway,
                transactionReference);
            
            // Record the payment in the Accommodation context
            // Use ProcessPartialPayment instead of RecordInstallmentPayment which doesn't exist
            reservation.ProcessPartialPayment(
                transaction.Id,
                amount);
            
            await _reservationRepository.UpdateAsync(reservation);
            
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
            // Validate that the reservation exists
            var reservation = await _reservationRepository.GetByIdAsync(reservationId);
            
            if (reservation == null)
                throw new InvalidOperationException($"Reservation with ID {reservationId} not found");
            
            // Process the foreign currency payment
            var transaction = await _paymentDomainService.ProcessForeignCurrencyPaymentAsync(
                reservationId,
                customerId,
                foreignAmount,
                localAmount,
                exchangeRate,
                paymentMethod,
                gateway);
            
            // Record the payment in the Accommodation context using the local amount
            reservation.ProcessPartialPayment(
                transaction.Id,
                localAmount);
            
            await _reservationRepository.UpdateAsync(reservation);
            
            return transaction;
        }
        
        #endregion
        
        #region Dispute Handling
        
        /// <summary>
        /// Handles a dispute filed against a payment transaction.
        /// </summary>
        public async Task<PaymentTransaction> HandlePaymentDisputeAsync(
            Guid transactionId,
            string disputeReason)
        {
            var transaction = await _paymentRepository.GetByIdAsync(transactionId);
            
            if (transaction == null)
                throw new InvalidOperationException($"Payment transaction with ID {transactionId} not found");
            
            // Record the dispute in the Payment context
            var updatedTransaction = await _paymentDomainService.RecordDisputeAsync(
                transactionId,
                disputeReason);
            
            // Update the reservation in the Accommodation context
            var reservation = await _reservationRepository.GetByIdAsync(transaction.ReservationId);
            
            if (reservation == null)
                throw new InvalidOperationException($"Reservation with ID {transaction.ReservationId} not found");
            
            // Use CancelWithReason since there's no specific MarkPaymentDisputed method
            reservation.CancelWithReason($"Payment disputed: {disputeReason}");
            
            await _reservationRepository.UpdateAsync(reservation);
            
            return updatedTransaction;
        }
        
        /// <summary>
        /// Resolves a payment dispute with the given outcome.
        /// </summary>
        public async Task<PaymentTransaction> ResolvePaymentDisputeAsync(
            Guid transactionId,
            bool inMerchantFavor,
            string resolutionDetails)
        {
            var transaction = await _paymentRepository.GetByIdAsync(transactionId);
            
            if (transaction == null)
                throw new InvalidOperationException($"Payment transaction with ID {transactionId} not found");
            
            // Resolve the dispute in the Payment context
            var updatedTransaction = await _paymentDomainService.ResolveDisputeAsync(
                transactionId,
                inMerchantFavor,
                resolutionDetails);
            
            // Update the reservation in the Accommodation context
            var reservation = await _reservationRepository.GetByIdAsync(transaction.ReservationId);
            
            if (reservation == null)
                throw new InvalidOperationException($"Reservation with ID {transaction.ReservationId} not found");
            
            if (inMerchantFavor)
            {
                // If resolved in merchant's favor, process the payment again
                reservation.ProcessPartialPayment(
                    transaction.Id,
                    transaction.Amount);
            }
            else
            {
                // If resolved in customer's favor, process a refund
                if (transaction.Amount != null)
                {
                    var refundTransaction = await _paymentDomainService.ProcessRefundAsync(
                        transactionId,
                        transaction.Amount);
                        
                    reservation.RecordRefund(
                        refundTransaction.Id,
                        transaction.Amount);
                }
            }
            
            await _reservationRepository.UpdateAsync(reservation);
            
            return updatedTransaction;
        }
        
        #endregion
        
        #region Fraud Handling
        
        /// <summary>
        /// Marks a payment transaction for additional verification due to suspected fraud.
        /// </summary>
        public async Task<PaymentTransaction> MarkPaymentForVerificationAsync(
            Guid transactionId, 
            string verificationReason)
        {
            var transaction = await _paymentRepository.GetByIdAsync(transactionId);
            
            if (transaction == null)
                throw new InvalidOperationException($"Payment transaction with ID {transactionId} not found");
            
            // Mark for verification in the Payment context
            var updatedTransaction = await _paymentDomainService.MarkPaymentForVerificationAsync(
                transactionId,
                verificationReason);
            
            // Update the reservation in the Accommodation context
            var reservation = await _reservationRepository.GetByIdAsync(transaction.ReservationId);
            
            if (reservation == null)
                throw new InvalidOperationException($"Reservation with ID {transaction.ReservationId} not found");
            
            // Mark the payment as failed in the Accommodation context
            reservation.MarkPaymentAsFailed(
                transactionId,
                $"Payment under verification: {verificationReason}");
            
            await _reservationRepository.UpdateAsync(reservation);
            
            return updatedTransaction;
        }
        
        /// <summary>
        /// Rejects a payment due to fraud detection.
        /// </summary>
        public async Task<PaymentTransaction> RejectPaymentAsFraudulentAsync(
            Guid transactionId, 
            string fraudReason)
        {
            var transaction = await _paymentRepository.GetByIdAsync(transactionId);
            
            if (transaction == null)
                throw new InvalidOperationException($"Payment transaction with ID {transactionId} not found");
            
            // Reject as fraudulent in the Payment context
            var updatedTransaction = await _paymentDomainService.RejectPaymentAsFraudulentAsync(
                transactionId,
                fraudReason);
            
            // Update the reservation in the Accommodation context
            var reservation = await _reservationRepository.GetByIdAsync(transaction.ReservationId);
            
            if (reservation == null)
                throw new InvalidOperationException($"Reservation with ID {transaction.ReservationId} not found");
            
            // Mark the payment as failed with fraud reason
            reservation.MarkPaymentAsFailed(
                transactionId,
                $"Payment rejected as fraudulent: {fraudReason}");
            
            await _reservationRepository.UpdateAsync(reservation);
            
            return updatedTransaction;
        }
        
        #endregion
        
        #region Payment Retry
        
        /// <summary>
        /// Creates a new payment attempt for a previously failed payment.
        /// </summary>
        public async Task<PaymentTransaction> RetryFailedPaymentAsync(
            Guid failedTransactionId,
            PaymentMethod? newPaymentMethod = null,
            string? newGateway = null)
        {
            var failedTransaction = await _paymentRepository.GetByIdAsync(failedTransactionId);
            
            if (failedTransaction == null)
                throw new InvalidOperationException($"Failed payment transaction with ID {failedTransactionId} not found");
            
            // Retry the payment in the Payment context
            var newTransaction = await _paymentDomainService.RetryFailedPaymentAsync(
                failedTransactionId,
                newPaymentMethod,
                newGateway);
            
            // Update the reservation in the Accommodation context
            var reservation = await _reservationRepository.GetByIdAsync(failedTransaction.ReservationId);
            
            if (reservation == null)
                throw new InvalidOperationException($"Reservation with ID {failedTransaction.ReservationId} not found");
            
            // No specific method for retry in Reservation, so we'll just mark a payment as pending
            reservation.MarkPaymentPending();
            
            await _reservationRepository.UpdateAsync(reservation);
            
            return newTransaction;
        }
        
        #endregion
    }
}
