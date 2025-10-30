using MassTransit;
using Nezam.Refahi.Contracts.Finance.v1.Messages;

namespace Nezam.Refahi.Orchestrator.Sagas.WalletDeposit;

/// <summary>
/// Saga state for orchestrating wallet deposit flow
/// Entry: WalletDepositRequestedIntegrationEvent → CreateBillIntegrationEvent
/// Progress: BillCreatedIntegrationEvent (ReferenceType == "WalletDeposit") → WalletDepositReadyToChargeIntegrationEvent
/// </summary>
public class WalletDepositSagaState : SagaStateMachineInstance, ISagaVersion
{
    public Guid CorrelationId { get; set; }
    public int Version { get; set; }
    public string CurrentState { get; set; } = string.Empty;

    // Deposit request
    public Guid ExternalUserId { get; set; }
    public string UserFullName { get; set; } = string.Empty;
    public decimal AmountRials { get; set; }
    public string Currency { get; set; } = "IRR";
    public string? Description { get; set; }
    public string ReferenceType { get; set; } = string.Empty;
    public string TrackingCode { get; set; } = string.Empty;
    public Guid WalletDepositId { get; set; }
    // Bill
    public Guid? BillId { get; set; }
    public string? BillNumber { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? BillCreatedAt { get; set; }
}

public class WalletDepositSagaStateMachine : MassTransitStateMachine<WalletDepositSagaState>
{
    public WalletDepositSagaStateMachine()
    {
        InstanceState(x => x.CurrentState);

        Event(() => WalletDepositRequested, x => x.CorrelateById(context => context.Message.WalletDepositId));
        Event(() => BillCreated, x => x.CorrelateById(context => context.Message.ReferenceId));
        Event(() => PendingFailed, x => x.CorrelateById(context => context.Message.WalletDepositId));
        Event(() => CompletionFailed, x => x.CorrelateById(context => context.Message.WalletDepositId));

        Initially(
            When(WalletDepositRequested)
                .Then(context =>
                {
                    context.Saga.CorrelationId = context.Message.WalletDepositId;
                    context.Saga.ExternalUserId = context.Message.ExternalUserId;
                    context.Saga.UserFullName = context.Message.UserFullName ?? string.Empty;
                    context.Saga.AmountRials = context.Message.AmountRials;
                    context.Saga.Currency = context.Message.Currency ?? "IRR";
                    context.Saga.Description = context.Message.Description;
                    context.Saga.TrackingCode = context.Message.TrackingCode;
                    context.Saga.WalletDepositId = context.Message.WalletDepositId;
                    context.Saga.ReferenceType = "WalletDeposit";
                    context.Saga.CreatedAt = DateTime.UtcNow;
                })
                .Send(context => new IssueBillCommandMessage
                {
                    TrackingCode = context.Saga.TrackingCode,
                    ReferenceId = context.Saga.WalletDepositId.ToString(), // correlate via tracking
                    ReferenceType = context.Saga.ReferenceType,
                    ExternalUserId = context.Saga.ExternalUserId,
                    UserFullName = context.Saga.UserFullName,
                    BillTitle = $"واریز کیف پول - {context.Saga.UserFullName}",
                    Description = context.Saga.Description ?? "ایجاد صورت‌حساب واریز کیف پول",
                    Metadata = new Dictionary<string, string>()
                    {
                        ["WalletDepositId"] = context.Saga.WalletDepositId.ToString()
                    },
                    Items = new List<CreateBillItemMessage>
                    {
                        new CreateBillItemMessage
                        {
                            Title = "واریز کیف پول",
                            Description = $"واریز کیف پول برای {context.Saga.UserFullName} - کد پیگیری: {context.Saga.TrackingCode}",
                            UnitPriceRials = context.Saga.AmountRials,
                            Quantity = 1,
                            DiscountPercentage = null
                        }
                    }
                })
                .TransitionTo(AwaitingBillCreation)
        );

        During(AwaitingBillCreation,
            When(BillCreated, context =>
                context.Message.ReferenceType == context.Saga.ReferenceType &&
                context.Message.ReferenceId == context.Saga.WalletDepositId)
                .Then(context =>
                {
                    context.Saga.BillId = context.Message.BillId;
                    context.Saga.BillNumber = context.Message.BillNumber;
                    context.Saga.BillCreatedAt = DateTime.UtcNow;
                })
                .Send(context => new MarkWalletDepositPendingCommandMessage
                {
                    WalletDepositId = context.Saga.WalletDepositId,
                    TrackingCode = context.Saga.TrackingCode,
                    ExternalUserId = context.Saga.ExternalUserId,
                    UserFullName = context.Saga.UserFullName,
                    AmountRials = context.Saga.AmountRials,
                    Currency = context.Saga.Currency,
                    BillId = context.Saga.BillId ?? context.Message.BillId,
                    BillNumber = context.Saga.BillNumber ?? context.Message.BillNumber
                })
                .TransitionTo(AwaitingPayment)
            ,
            When(PendingFailed)
                .Send(context => new FailWalletDepositCommandMessage
                {
                    WalletDepositId = context.Saga.WalletDepositId,
                    TrackingCode = context.Saga.TrackingCode,
                    FailureStage = "Pending",
                    FailureReason = context.Message.FailureReason,
                    ErrorCode = context.Message.ErrorCode,
                    BillId = context.Saga.BillId,
                    BillNumber = context.Saga.BillNumber
                })
                .TransitionTo(Failed)
        );

        // Listen for bill fully paid to complete the saga when payment succeeds
        Event(() => BillFullyPaid, x => x.CorrelateBy((saga, context) => saga.WalletDepositId == context.Message.ReferenceId));

        During(AwaitingPayment,
            When(BillFullyPaid, context =>
                context.Message.ReferenceType == context.Saga.ReferenceType &&
                context.Message.ReferenceId == context.Saga.WalletDepositId)
                .Send(context => new CompleteWalletDepositCommandMessage
                {
                    WalletDepositId = context.Saga.WalletDepositId,
                    TrackingCode = context.Saga.TrackingCode,
                    ExternalUserId = context.Saga.ExternalUserId,
                    UserFullName = context.Saga.UserFullName,
                    AmountRials = context.Saga.AmountRials,
                    Currency = context.Saga.Currency,
                    BillId = context.Saga.BillId ?? Guid.Empty,
                    BillNumber = context.Saga.BillNumber ?? string.Empty,
                    PaymentId = context.Message.PaymentId,
                    PaidAt = context.Message.PaidAt
                })
                .Finalize()
            ,
            When(CompletionFailed)
                .Send(context => new FailWalletDepositCommandMessage
                {
                    WalletDepositId = context.Saga.WalletDepositId,
                    TrackingCode = context.Saga.TrackingCode,
                    FailureStage = "Completion",
                    FailureReason = context.Message.FailureReason,
                    ErrorCode = context.Message.ErrorCode,
                    BillId = context.Saga.BillId,
                    BillNumber = context.Saga.BillNumber,
                    PaymentId = context.Message.PaymentId
                })
                .TransitionTo(Failed)
        );
    }

    public State AwaitingBillCreation { get; private set; } = null!;
    public State AwaitingPayment { get; private set; } = null!;
    public State Failed { get; private set; } = null!;

    public Event<WalletDepositRequestedEventMessage> WalletDepositRequested { get; private set; } = null!;
    public Event<BillCreatedEventMessage> BillCreated { get; private set; } = null!;
    public Event<BillFullyPaidEventMessage> BillFullyPaid { get; private set; } = null!;
    public Event<WalletDepositPendingFailedEventMessage> PendingFailed { get; private set; } = null!;
    public Event<WalletDepositCompletionFailedMessage> CompletionFailed { get; private set; } = null!;
}


