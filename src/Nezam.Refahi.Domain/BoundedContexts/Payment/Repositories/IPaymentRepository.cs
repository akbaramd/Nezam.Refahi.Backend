using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nezam.Refahi.Domain.BoundedContexts.Payment.Aggregates;

namespace Nezam.Refahi.Domain.BoundedContexts.Payment.Repositories
{
    /// <summary>
    /// Repository interface for accessing and persisting payment transactions.
    /// </summary>
    public interface IPaymentRepository
    {
        Task<PaymentTransaction?> GetByIdAsync(Guid id);
        Task<PaymentTransaction?> GetByTransactionReferenceAsync(string transactionReference);
        Task<IEnumerable<PaymentTransaction>> GetByReservationIdAsync(Guid reservationId);
        Task<IEnumerable<PaymentTransaction>> GetByCustomerIdAsync(Guid customerId);
        Task<IEnumerable<PaymentTransaction>> GetPendingTransactionsAsync();
        Task<IEnumerable<PaymentTransaction>> GetCompletedTransactionsInDateRangeAsync(DateTimeOffset startDate, DateTimeOffset endDate);
        Task AddAsync(PaymentTransaction paymentTransaction);
        Task UpdateAsync(PaymentTransaction paymentTransaction);
        Task<bool> DeleteAsync(Guid id);
        Task<decimal> GetTotalAmountByReservationIdAsync(Guid reservationId);
    }
}
