# 🔄 Finance Domain Business Flows

## 📋 Overview

This document describes the complete business flows within the Finance Domain Context, detailing how different entities interact and how business processes are executed.

## 🏦 Core Business Flows

### 1. 💰 Wallet Charging Flow

#### Description
The process of adding money to a user's wallet through bill payment.

#### Flow Steps
```
1. User Request
   ↓
2. Create WalletDeposit
   ↓
3. Create Bill
   ↓
4. Link Deposit to Bill
   ↓
5. User Payment
   ↓
6. Payment Completion
   ↓
7. Deposit Completion
   ↓
8. Wallet Charging
   ↓
9. Transaction Recording
```

#### Detailed Process

**Step 1: User Request**
- User requests to charge their wallet
- System validates user and wallet status
- Amount and payment method are specified

**Step 2: Create WalletDeposit**
```csharp
var walletDeposit = new WalletDeposit(
    walletId: wallet.Id,
    userNationalNumber: userNationalNumber,
    amount: Money.FromRials(amountRials),
    description: "Wallet charging deposit"
);
// Status: Pending
```

**Step 3: Create Bill**
```csharp
var bill = new Bill(
    title: $"Wallet Deposit - {userFullName}",
    referenceId: walletDeposit.Id.ToString(),
    billType: "WalletDeposit",
    userNationalNumber: userNationalNumber,
    userFullName: userFullName,
    description: "Wallet depositing bill",
    dueDate: DateTime.UtcNow.AddDays(7)
);
// Status: Draft
```

**Step 4: Link Deposit to Bill**
```csharp
walletDeposit.SetBillId(bill.Id);
// Deposit now references the bill
```

**Step 5: User Payment**
- User initiates payment through payment gateway
- Payment is processed through external gateway
- Payment status is tracked

**Step 6: Payment Completion**
- Payment is completed successfully
- `BillFullyPaidEvent` is raised
- Bill status changes to `Paid`

**Step 7: Deposit Completion**
```csharp
// In WalletChargePaymentCompletedConsumer
deposit.Complete();
// Status: Pending → Completed
// CompletedAt is set
```

**Step 8: Wallet Charging**
```csharp
var chargeCommand = new ChargeWalletCommand
{
    UserNationalNumber = userNationalNumber,
    AmountRials = deposit.Amount.AmountRials,
    ReferenceId = deposit.Id.ToString(),
    Description = "Wallet deposit completion"
};
```

**Step 9: Transaction Recording**
```csharp
var transaction = wallet.Deposit(
    amount: deposit.Amount,
    referenceId: deposit.Id.ToString(),
    description: "Wallet deposit completion",
    externalReference: payment.GatewayTransactionId
);
// WalletTransaction created with Status: Completed
```

#### Events Raised
- `WalletDepositRequestedEvent`
- `BillCreatedEvent`
- `BillStatusChangedEvent`
- `PaymentInitiatedEvent`
- `PaymentCompletedEvent`
- `BillFullyPaidEvent`
- `WalletDepositCompletedEvent`
- `WalletBalanceChangedEvent`
- `WalletTransactionCompletedEvent`

### 2. 🧾 Bill Payment Flow

#### Description
The process of paying a bill through various payment methods.

#### Flow Steps
```
1. Bill Creation
   ↓
2. Bill Items Addition
   ↓
3. Bill Issuance
   ↓
4. Payment Initiation
   ↓
5. Payment Processing
   ↓
6. Payment Completion
   ↓
7. Bill Status Update
   ↓
8. Related Actions
```

#### Detailed Process

**Step 1: Bill Creation**
```csharp
var bill = new Bill(
    title: "Service Fee",
    referenceId: "SERVICE_001",
    billType: "ServiceFee",
    userNationalNumber: userNationalNumber,
    userFullName: userFullName,
    description: "Monthly service fee",
    dueDate: DateTime.UtcNow.AddDays(30)
);
// Status: Draft
```

**Step 2: Bill Items Addition**
```csharp
bill.AddItem(
    title: "Service Fee",
    description: "Monthly service subscription",
    unitPrice: Money.FromRials(100000),
    quantity: 1,
    discountPercentage: 0
);
```

**Step 3: Bill Issuance**
```csharp
bill.Issue();
// Status: Draft → Issued
// IssueDate is set
```

**Step 4: Payment Initiation**
```csharp
var payment = new Payment(
    billId: bill.Id,
    amount: bill.TotalAmount,
    paymentMethod: PaymentMethod.Card,
    gateway: PaymentGateway.Parsian,
    expiresAt: DateTime.UtcNow.AddHours(24)
);
// Status: Initiated
```

**Step 5: Payment Processing**
```csharp
payment.Process();
// Status: Initiated → Processing
// Gateway processing begins
```

**Step 6: Payment Completion**
```csharp
payment.Complete(gatewayTransactionId: "GATEWAY_123");
// Status: Processing → Completed
// CompletedAt is set
```

**Step 7: Bill Status Update**
```csharp
bill.MarkAsPaid(payment.Amount);
// Status: Issued → Paid
// PaidAmount is updated
// RemainingAmount is calculated
```

**Step 8: Related Actions**
- If bill is fully paid: `BillFullyPaidEvent` is raised
- If bill is wallet deposit: Wallet charging process begins
- If bill is overdue: `BillOverdueEvent` is raised

#### Events Raised
- `BillCreatedEvent`
- `BillStatusChangedEvent`
- `PaymentInitiatedEvent`
- `PaymentProcessingEvent`
- `PaymentCompletedEvent`
- `BillFullyPaidEvent` (if fully paid)
- `BillOverdueEvent` (if overdue)

### 3. 💸 Refund Processing Flow

#### Description
The process of refunding a completed payment.

#### Flow Steps
```
1. Refund Request
   ↓
2. Refund Validation
   ↓
3. Refund Initiation
   ↓
4. Refund Processing
   ↓
5. Refund Completion
   ↓
6. Wallet Credit (if applicable)
   ↓
7. Transaction Recording
```

#### Detailed Process

**Step 1: Refund Request**
- User or system requests refund
- Original payment is identified
- Refund amount is specified
- Refund reason is provided

**Step 2: Refund Validation**
```csharp
// Validate refund eligibility
if (payment.Status != PaymentStatus.Completed)
    throw new InvalidOperationException("Cannot refund incomplete payment");

if (refundAmount > payment.Amount)
    throw new ArgumentException("Refund amount cannot exceed payment amount");
```

**Step 3: Refund Initiation**
```csharp
var refund = new Refund(
    paymentId: payment.Id,
    amount: refundAmount,
    reason: "User requested refund"
);
// Status: Initiated
```

**Step 4: Refund Processing**
```csharp
refund.Process();
// Status: Initiated → Processing
// Gateway refund processing begins
```

**Step 5: Refund Completion**
```csharp
refund.Complete();
// Status: Processing → Completed
// CompletedAt is set
```

**Step 6: Wallet Credit (if applicable)**
```csharp
if (bill.BillType == "WalletDeposit")
{
    // Credit the wallet
    var wallet = await _walletRepository.GetByNationalNumberAsync(userNationalNumber);
    var transaction = wallet.Withdraw(
        amount: refund.Amount,
        referenceId: refund.Id.ToString(),
        description: "Refund from deposit",
        externalReference: refund.Id.ToString()
    );
}
```

**Step 7: Transaction Recording**
```csharp
// Refund transaction is recorded
// Original payment status may be updated
// Audit trail is maintained
```

#### Events Raised
- `RefundInitiatedEvent`
- `RefundProcessingEvent`
- `RefundCompletedEvent`
- `WalletBalanceChangedEvent` (if wallet credited)
- `WalletTransactionCompletedEvent` (if wallet credited)

### 4. 🔄 Wallet Transfer Flow

#### Description
The process of transferring money between wallets.

#### Flow Steps
```
1. Transfer Request
   ↓
2. Validation
   ↓
3. Source Wallet Debit
   ↓
4. Target Wallet Credit
   ↓
5. Transaction Recording
   ↓
6. Event Notification
```

#### Detailed Process

**Step 1: Transfer Request**
- User specifies source and target wallets
- Transfer amount is specified
- Transfer reason is provided

**Step 2: Validation**
```csharp
// Validate wallets exist and are active
if (sourceWallet.Status != WalletStatus.Active)
    throw new InvalidOperationException("Source wallet is not active");

if (targetWallet.Status != WalletStatus.Active)
    throw new InvalidOperationException("Target wallet is not active");

// Validate sufficient balance
if (sourceWallet.Balance.AmountRials < transferAmount)
    throw new InvalidOperationException("Insufficient balance");
```

**Step 3: Source Wallet Debit**
```csharp
var sourceTransaction = sourceWallet.Withdraw(
    amount: Money.FromRials(transferAmount),
    referenceId: transferId.ToString(),
    description: $"Transfer to {targetWallet.NationalNumber}",
    externalReference: transferId.ToString()
);
```

**Step 4: Target Wallet Credit**
```csharp
var targetTransaction = targetWallet.Deposit(
    amount: Money.FromRials(transferAmount),
    referenceId: transferId.ToString(),
    description: $"Transfer from {sourceWallet.NationalNumber}",
    externalReference: transferId.ToString()
);
```

**Step 5: Transaction Recording**
```csharp
// Both transactions are recorded
// Transfer is marked as completed
// Audit trail is maintained
```

**Step 6: Event Notification**
```csharp
// Events are raised for both wallets
// Transfer completion is notified
// Related systems are updated
```

#### Events Raised
- `WalletBalanceChangedEvent` (for both wallets)
- `WalletTransactionCompletedEvent` (for both wallets)
- `TransferCompletedEvent` (if implemented)

## 🎯 Business Rules Enforcement

### 1. Wallet Rules
- **Unique Wallet**: One wallet per national number
- **Non-negative Balance**: Balance cannot go below zero
- **Transaction Recording**: All operations create transactions
- **Status Validation**: Status affects capabilities

### 2. Bill Rules
- **Multiple Items**: Bills can have multiple items
- **Partial Payments**: Bills support partial payments
- **Overdue Detection**: Bills become overdue after due date
- **Cancellation**: Bills can be cancelled if not paid

### 3. Payment Rules
- **Bill Association**: Payments must be linked to bills
- **Gateway Integration**: Multiple payment gateways supported
- **Status Tracking**: Payment status tracked throughout lifecycle
- **Retry Capability**: Failed payments can be retried

### 4. Refund Rules
- **Payment Validation**: Only completed payments can be refunded
- **Amount Validation**: Refund amount cannot exceed payment amount
- **Status Tracking**: Refund status tracked throughout lifecycle
- **Wallet Credit**: Refunds can credit wallets if applicable

## 🔧 Domain Service Coordination

### WalletDomainService
The `WalletDomainService` coordinates complex business scenarios:

```csharp
public sealed class WalletDomainService
{
    // Analyze wallet balance history
    public WalletBalanceAnalysis AnalyzeBalanceHistory(
        Wallet wallet, 
        DateTime? fromDate = null, 
        DateTime? toDate = null);

    // Validate transaction limits
    public TransactionLimitValidation ValidateTransactionLimits(
        Wallet wallet, 
        Money amount, 
        WalletTransactionType transactionType);

    // Calculate transaction fees
    public Money CalculateFees(Money amount, WalletTransactionType transactionType);

    // Process wallet-to-wallet transfer
    public TransferResult ProcessTransfer(
        Wallet sourceWallet, 
        Wallet targetWallet, 
        Money amount);

    // Validate payment from wallet
    public PaymentValidation ValidatePayment(
        Wallet wallet, 
        Money amount, 
        PaymentMethod paymentMethod);
}
```

## 📊 Event-Driven Architecture

### Event Flow
All business processes are event-driven:

1. **Domain Events**: Raised when aggregate state changes
2. **Event Handlers**: Process events and coordinate actions
3. **Integration Events**: Communicate with other bounded contexts
4. **Event Sourcing**: Maintain complete event history

### Event Types
- **Aggregate Events**: Raised by aggregates
- **Domain Events**: Raised by domain services
- **Integration Events**: Raised for external communication
- **Application Events**: Raised by application services

## 🔐 Security & Compliance

### Data Integrity
- **Immutable Transactions**: Financial transactions cannot be modified
- **Audit Trail**: Complete history of all changes
- **Validation**: Business rules enforced at domain level
- **Consistency**: Aggregate boundaries ensure consistency

### Security Measures
- **Access Control**: Repository pattern provides access abstraction
- **Data Encryption**: Sensitive data encrypted
- **Audit Logging**: All operations logged
- **Transaction Limits**: Limits enforced at domain level

## 📈 Performance Considerations

### Optimization Strategies
- **Repository Separation**: Different repositories for different entities
- **Query Optimization**: Efficient queries for common operations
- **Indexing Strategy**: Proper database indexing
- **Caching**: Frequently accessed data cached

### Scalability Design
- **Aggregate Boundaries**: Clear consistency boundaries
- **Event-Driven**: Loose coupling through events
- **Repository Pattern**: Clean data access abstraction
- **Domain Services**: Stateless business logic coordination

---

This document provides a comprehensive overview of all business flows within the Finance Domain Context, ensuring proper understanding and implementation of financial processes.
