using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nezam.Refahi.Domain.BoundedContexts.Payment.Aggregates;
using Nezam.Refahi.Domain.BoundedContexts.Shared.Repositories;

namespace Nezam.Refahi.Domain.BoundedContexts.Payment.Repositories
{
    /// <summary>
    /// Repository interface for accessing and persisting payment transactions.
    /// </summary>
    public interface IPaymentRepository : IGenericRepository<PaymentTransaction>
    {
        Task<PaymentTransaction?> GetByTransactionReferenceAsync(string transactionReference);
        Task<IEnumerable<PaymentTransaction>> GetByReservationIdAsync(Guid reservationId);
        Task<IEnumerable<PaymentTransaction>> GetByCustomerIdAsync(Guid customerId);
        Task<IEnumerable<PaymentTransaction>> GetPendingTransactionsAsync();
        Task<IEnumerable<PaymentTransaction>> GetCompletedTransactionsInDateRangeAsync(DateTimeOffset startDate, DateTimeOffset endDate);
        Task<decimal> GetTotalAmountByReservationIdAsync(Guid reservationId);
    }
}
