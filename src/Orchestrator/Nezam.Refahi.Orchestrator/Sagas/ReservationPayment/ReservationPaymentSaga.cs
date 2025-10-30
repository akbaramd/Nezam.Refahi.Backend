using MassTransit;
using MassTransit.Saga;
using Nezam.Refahi.Finance.Contracts.IntegrationEvents;
using Nezam.Refahi.Recreation.Contracts.IntegrationEvents;

namespace Nezam.Refahi.Orchestrator.Sagas.ReservationPayment;

/// <summary>
/// Saga state for orchestrating reservation payment flow
/// Coordinates the payment process between Recreation and Finance modules
/// </summary>
public class ReservationPaymentSagaState : SagaStateMachineInstance, ISagaVersion
{
    public Guid CorrelationId { get; set; }
    public int Version { get; set; }
    public string CurrentState { get; set; } = string.Empty;

    // Reservation details
    public Guid ReservationId { get; set; }
    public string TrackingCode { get; set; } = string.Empty;
    public Guid TourId { get; set; }
    public string TourTitle { get; set; } = string.Empty;

    // User details
    public Guid ExternalUserId { get; set; }
    public string UserFullName { get; set; } = string.Empty;

    // Payment details
    public decimal TotalAmountRials { get; set; }
    public string Currency { get; set; } = "IRR";

    // Bill details
    public Guid? BillId { get; set; }
    public string? BillNumber { get; set; }

    // Payment details
    public Guid? PaymentId { get; set; }
    public string? GatewayTransactionId { get; set; }
    public string? GatewayReference { get; set; }
    public string? Gateway { get; set; }

    // Failure details
    public string? FailureReason { get; set; }
    public string? ErrorCode { get; set; }

    // Timestamps
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? BillCreatedAt { get; set; }
    public DateTime? PaymentCompletedAt { get; set; }
    public DateTime? PaymentFailedAt { get; set; }
}

/// <summary>
/// Saga state machine for reservation payment orchestration
/// </summary>
public class ReservationPaymentSagaStateMachine : MassTransitStateMachine<ReservationPaymentSagaState>
{
    public ReservationPaymentSagaStateMachine()
    {
        InstanceState(x => x.CurrentState);

        // Reservation held event (single entry path)
        Event(() => ReservationHeld, x => x.CorrelateById(context => context.Message.ReservationId));

        // Bill creation events
        Event(() => BillCreated, x => x.CorrelateById(context => context.Message.ReferenceId));

        // Payment completion events
        Event(() => PaymentCompleted, x => x.CorrelateById(context => 
            Guid.TryParse(context.Message.ReferenceId, out var reservationId) && 
            context.Message.ReferenceType == "TourReservation" ? reservationId : Guid.Empty));

        Event(() => PaymentFailed, x => x.CorrelateById(context => 
            Guid.TryParse(context.Message.ReferenceId, out var reservationId) && 
            context.Message.ReferenceType == "TourReservation" ? reservationId : Guid.Empty));

        // Start from initial state (single entry path)
        Initially(
            When(ReservationHeld)
                .Then(context =>
                {
                    context.Saga.ReservationId = context.Message.ReservationId;
                    context.Saga.TrackingCode = context.Message.TrackingCode;
                    context.Saga.TourId = context.Message.TourId;
                    context.Saga.TourTitle = context.Message.TourTitle;
                    context.Saga.ExternalUserId = context.Message.ExternalUserId;
                    context.Saga.UserFullName = context.Message.UserFullName;
                    context.Saga.TotalAmountRials = context.Message.TotalAmountRials;
                    context.Saga.Currency = context.Message.Currency;
                    context.Saga.CreatedAt = DateTime.UtcNow;
                })
                .Publish(context => new CreateBillIntegrationEvent
                {
                    TrackingCode = context.Saga.TrackingCode,
                    ReferenceId = context.Saga.ReservationId.ToString(),
                    ReferenceType = "TourReservation",
                    ExternalUserId = context.Saga.ExternalUserId,
                    UserFullName = context.Saga.UserFullName,
                    AmountRials = context.Saga.TotalAmountRials,
                    Currency = context.Saga.Currency,
                    BillTitle = $"فاکتور تور {context.Saga.TourTitle}",
                    Description = $"ایجاد فاکتور برای رزرو: {context.Saga.TrackingCode}",
                    Metadata = new Dictionary<string, string>
                    {
                        ["ReservationId"] = context.Saga.ReservationId.ToString(),
                        ["TrackingCode"] = context.Saga.TrackingCode,
                        ["TourId"] = context.Saga.TourId.ToString(),
                        ["TourTitle"] = context.Saga.TourTitle
                    }
                })
                .TransitionTo(AwaitingBillCreation)
        );

        // Bill creation received
        During(AwaitingBillCreation,
            When(BillCreated, context =>
                context.Message.Metadata != null &&
                context.Message.Metadata.TryGetValue("ReferenceType", out var refType) && refType == "TourReservation" &&
                context.Message.Metadata.TryGetValue("ReferenceId", out var refId) && refId == context.Saga.ReservationId.ToString())
                .Then(context =>
                {
                    context.Saga.BillId = context.Message.BillId;
                    context.Saga.BillNumber = context.Message.BillNumber;
                    context.Saga.BillCreatedAt = DateTime.UtcNow;
                })
                .Publish(context => new ReservationReadyToCompleteIntegrationEvent
                {
                    ReservationId = context.Saga.ReservationId,
                    TrackingCode = context.Saga.TrackingCode,
                    BillId = context.Saga.BillId ?? context.Message.BillId,
                    BillNumber = context.Saga.BillNumber ?? context.Message.BillNumber,
                    TotalAmountRials = context.Saga.TotalAmountRials,
                    Currency = context.Saga.Currency,
                    ExternalUserId = context.Saga.ExternalUserId,
                    UserFullName = context.Saga.UserFullName,
                    EventId = Guid.NewGuid(),
                    OccurredOn = DateTime.UtcNow,
                    EventVersion = "1.0",
                    Metadata = new Dictionary<string, string>
                    {
                        ["ReferenceId"] = context.Saga.ReservationId.ToString(),
                        ["ReferenceType"] = "TourReservation"
                    }
                })
                .TransitionTo(AwaitingPayment)
        );

        // Payment completed successfully
        During(AwaitingPayment,
            When(PaymentCompleted)
                .Then(context =>
                {
                    context.Saga.PaymentId = context.Message.PaymentId;
                    context.Saga.GatewayTransactionId = context.Message.GatewayTransactionId;
                    context.Saga.GatewayReference = context.Message.GatewayReference;
                    context.Saga.PaymentCompletedAt = DateTime.UtcNow;
                })
                .Publish(context => new ReservationPaymentCompletedIntegrationEvent
                {
                    ReservationId = context.Saga.ReservationId,
                    TrackingCode = context.Saga.TrackingCode,
                    PaymentId = context.Saga.PaymentId ?? context.Message.PaymentId,
                    BillId = context.Saga.BillId ?? Guid.Empty,
                    BillNumber = context.Saga.BillNumber ?? string.Empty,
                    AmountRials = context.Saga.TotalAmountRials,
                    PaidAt = context.Saga.PaymentCompletedAt ?? DateTime.UtcNow,
                    GatewayTransactionId = context.Saga.GatewayTransactionId,
                    GatewayReference = context.Saga.GatewayReference,
                    Gateway = context.Message.GatewayReference ?? string.Empty,
                    ExternalUserId = context.Saga.ExternalUserId,
                    EventId = Guid.NewGuid(),
                    OccurredOn = DateTime.UtcNow,
                    EventVersion = "1.0"
                })
                .Finalize()
        );

        // Payment failed
        During(AwaitingPayment,
            When(PaymentFailed)
                .Then(context =>
                {
                    context.Saga.PaymentId = context.Message.PaymentId;
                    context.Saga.FailureReason = context.Message.FailureReason;
                    context.Saga.ErrorCode = context.Message.ErrorCode;
                    context.Saga.GatewayTransactionId = context.Message.GatewayTransactionId;
                    context.Saga.PaymentFailedAt = DateTime.UtcNow;
                })
                .Publish(context => new ReservationPaymentFailedIntegrationEvent
                {
                    ReservationId = context.Saga.ReservationId,
                    TrackingCode = context.Saga.TrackingCode,
                    PaymentId = context.Saga.PaymentId,
                    BillId = context.Saga.BillId,
                    BillNumber = context.Saga.BillNumber,
                    AmountRials = context.Saga.TotalAmountRials,
                    FailedAt = context.Saga.PaymentFailedAt ?? DateTime.UtcNow,
                    FailureReason = context.Saga.FailureReason ?? string.Empty,
                    ErrorCode = context.Saga.ErrorCode,
                    GatewayTransactionId = context.Saga.GatewayTransactionId,
                    EventId = Guid.NewGuid(),
                    OccurredOn = DateTime.UtcNow,
                    EventVersion = "1.0"
                })
                .Finalize()
        );
    }

    // States
    public State AwaitingBillCreation { get; private set; } = null!;
    public State AwaitingPayment { get; private set; } = null!;
    public State Completed { get; private set; } = null!;
    public State Failed { get; private set; } = null!;

    // Events
    public Event<ReservationHeldIntegrationEvent> ReservationHeld { get; private set; } = null!;
    public Event<BillCreatedIntegrationEvent> BillCreated { get; private set; } = null!;
    public Event<PaymentCompletedIntegrationEvent> PaymentCompleted { get; private set; } = null!;
    public Event<PaymentFailedIntegrationEvent> PaymentFailed { get; private set; } = null!;
}

