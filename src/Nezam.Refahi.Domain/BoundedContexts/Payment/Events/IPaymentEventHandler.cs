using System.Threading.Tasks;

namespace Nezam.Refahi.Domain.BoundedContexts.Payment.Events
{
    /// <summary>
    /// Interface for handling payment events.
    /// This facilitates loose coupling between the Payment bounded context and other bounded contexts
    /// by using the Observer pattern for domain events.
    /// </summary>
    public interface IPaymentEventHandler
    {
        /// <summary>
        /// Handles payment completed events
        /// </summary>
        Task HandlePaymentCompletedAsync(PaymentCompletedEvent paymentEvent);
        
        /// <summary>
        /// Handles payment failed events
        /// </summary>
        Task HandlePaymentFailedAsync(PaymentFailedEvent paymentEvent);
    }
}
