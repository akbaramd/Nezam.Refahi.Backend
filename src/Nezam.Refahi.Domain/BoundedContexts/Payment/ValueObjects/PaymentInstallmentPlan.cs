using System;
using System.Collections.Generic;
using Nezam.Refahi.Domain.BoundedContexts.Accommodation.ValueObjects;
using Nezam.Refahi.Domain.BoundedContexts.Shared;

namespace Nezam.Refahi.Domain.BoundedContexts.Payment.ValueObjects
{
    /// <summary>
    /// Represents a payment installment plan with scheduled payment dates and amounts.
    /// Following DDD principles, this is a value object representing a concept with no identity,
    /// used within the payment context.
    /// </summary>
    public class PaymentInstallmentPlan
    {
        public Money TotalAmount { get; private set; }
        public int NumberOfInstallments { get; private set; }
        public IReadOnlyList<PaymentInstallment> Installments => _installments.AsReadOnly();
        
        private readonly List<PaymentInstallment> _installments = new List<PaymentInstallment>();
        
        
        public PaymentInstallmentPlan(Money totalAmount, int numberOfInstallments, DateTimeOffset firstPaymentDate, int daysBetweenPayments)
        {
            if (totalAmount == null || totalAmount.Amount <= 0)
                throw new ArgumentException("Total amount must be positive", nameof(totalAmount));
                
            if (numberOfInstallments <= 0)
                throw new ArgumentException("Number of installments must be positive", nameof(numberOfInstallments));
                
            if (daysBetweenPayments < 0)
                throw new ArgumentException("Days between payments cannot be negative", nameof(daysBetweenPayments));
                
            TotalAmount = totalAmount;
            NumberOfInstallments = numberOfInstallments;
            
            // Calculate installment amount (with rounding)
            decimal installmentAmount = Math.Round(totalAmount.Amount / numberOfInstallments, 2);
            decimal remainder = totalAmount.Amount - (installmentAmount * numberOfInstallments);
            
            // Generate installment schedule
            for (int i = 0; i < numberOfInstallments; i++)
            {
                decimal currentAmount = installmentAmount;
                if (i == numberOfInstallments - 1)
                {
                    // Add any remaining amount (due to rounding) to the last installment
                    currentAmount += remainder;
                }
                
                var dueDate = firstPaymentDate.AddDays(i * daysBetweenPayments);
                var installment = new PaymentInstallment(
                    new Money(currentAmount, totalAmount.Currency),
                    i + 1,
                    numberOfInstallments,
                    dueDate
                );
                
                _installments.Add(installment);
            }
        }
        
        public bool CanModify()
        {
            // Can modify only if no installments have been paid yet
            return !_installments.Exists(i => i.IsPaid);
        }
        
        public PaymentInstallment? GetNextUnpaidInstallment()
        {
            return _installments.Find(i => !i.IsPaid);
        }
        
        public Money GetRemainingAmount()
        {
            decimal totalPaid = 0;
            foreach (var installment in _installments)
            {
                if (installment.IsPaid)
                {
                    totalPaid += installment.Amount.Amount;
                }
            }
            
            return new Money(TotalAmount.Amount - totalPaid, TotalAmount.Currency);
        }
        
        public bool IsFullyPaid()
        {
            return _installments.TrueForAll(i => i.IsPaid);
        }
    }
    
    /// <summary>
    /// Represents a single installment within a payment plan.
    /// </summary>
    public class PaymentInstallment
    {
        public Money Amount { get; private set; }
        public int InstallmentNumber { get; private set; }
        public int TotalInstallments { get; private set; }
        public DateTimeOffset DueDate { get; private set; }
        public DateTimeOffset? PaidDate { get; private set; }
        public Guid? TransactionId { get; private set; }
        public bool IsPaid => TransactionId.HasValue;
    
        
        public PaymentInstallment(Money amount, int installmentNumber, int totalInstallments, DateTimeOffset dueDate)
        {
            Amount = amount ?? throw new ArgumentNullException(nameof(amount));
            
            if (installmentNumber <= 0)
                throw new ArgumentException("Installment number must be positive", nameof(installmentNumber));
                
            if (totalInstallments <= 0)
                throw new ArgumentException("Total installments must be positive", nameof(totalInstallments));
                
            if (installmentNumber > totalInstallments)
                throw new ArgumentException("Installment number cannot exceed total installments");
                
            InstallmentNumber = installmentNumber;
            TotalInstallments = totalInstallments;
            DueDate = dueDate;
        }
        
        public void MarkAsPaid(Guid transactionId, DateTimeOffset paidDate)
        {
            if (IsPaid)
                throw new InvalidOperationException("Installment has already been paid");
                
            TransactionId = transactionId;
            PaidDate = paidDate;
        }
        
        public bool IsOverdue(DateTimeOffset currentDate)
        {
            return !IsPaid && currentDate > DueDate;
        }
    }
}
