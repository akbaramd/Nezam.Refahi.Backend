using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Domain.BoundedContexts.Payment.Aggregates;
using Nezam.Refahi.Domain.BoundedContexts.Payment.Repositories;
using Nezam.Refahi.Domain.BoundedContexts.Payment.ValueObjects;

namespace Nezam.Refahi.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Implementation of payment repository interface using EF Core
    /// </summary>
    public class PaymentRepository : GenericRepository<PaymentTransaction>, IPaymentRepository
    {
        public PaymentRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<PaymentTransaction?> GetByTransactionReferenceAsync(string transactionReference)
        {
            if (string.IsNullOrEmpty(transactionReference))
                return null;
                
            return await AsDbSet()
                .FirstOrDefaultAsync(pt => pt.TransactionReference == transactionReference);
        }

        public async Task<IEnumerable<PaymentTransaction>> GetByReservationIdAsync(Guid reservationId)
        {
            return await AsDbSet()
                .Where(pt => pt.ReservationId == reservationId)
                .OrderByDescending(pt => pt.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<PaymentTransaction>> GetByCustomerIdAsync(Guid customerId)
        {
            return await AsDbSet()
                .Where(pt => pt.CustomerId == customerId)
                .OrderByDescending(pt => pt.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<PaymentTransaction>> GetPendingTransactionsAsync()
        {
            return await AsDbSet()
                .Where(pt => pt.Status == PaymentStatus.Pending || pt.Status == PaymentStatus.Authorized)
                .OrderBy(pt => pt.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<PaymentTransaction>> GetCompletedTransactionsInDateRangeAsync(
            DateTimeOffset startDate, 
            DateTimeOffset endDate)
        {
            return await AsDbSet()
                .Where(pt => pt.Status == PaymentStatus.Completed && 
                            pt.CompletionDate >= startDate && 
                            pt.CompletionDate <= endDate)
                .OrderByDescending(pt => pt.CompletionDate)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalAmountByReservationIdAsync(Guid reservationId)
        {
            // Get only successful payments for this reservation
            var successfulPayments = await AsDbSet()
                .Where(pt => pt.ReservationId == reservationId && 
                            pt.Status == PaymentStatus.Completed)
                .ToListAsync();
                
            // Sum the amounts (considering the Money value object)
            return successfulPayments.Sum(pt => pt.Amount.Amount);
        }
    }
}
