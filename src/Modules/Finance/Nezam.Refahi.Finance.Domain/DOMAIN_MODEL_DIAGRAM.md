# 🏗️ Finance Domain Model Diagram

## 📊 Entity Relationship Overview

```
┌─────────────────────────────────────────────────────────────────────────────────┐
│                              FINANCE DOMAIN CONTEXT                             │
└─────────────────────────────────────────────────────────────────────────────────┘

┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│      BILL       │    │     WALLET      │    │    PAYMENT      │    │     REFUND      │
│  (Aggregate)    │    │  (Aggregate)    │    │  (Aggregate)    │    │  (Aggregate)    │
├─────────────────┤    ├─────────────────┤    ├─────────────────┤    ├─────────────────┤
│ • BillNumber    │    │ • NationalNumber│    │ • BillId        │    │ • PaymentId     │
│ • Title         │    │ • Balance       │    │ • Amount        │    │ • Amount        │
│ • ReferenceId   │    │ • Status        │    │ • Status        │    │ • Status        │
│ • BillType      │    │ • WalletName    │    │ • PaymentMethod │    │ • Reason        │
│ • UserNational  │    │ • Description   │    │ • Gateway       │    │ • InitiatedAt   │
│ • Status        │    │ • LastTransAt   │    │ • GatewayTransId│    │ • CompletedAt   │
│ • TotalAmount   │    │ • Metadata       │    │ • InitiatedAt   │    │ • Metadata      │
│ • PaidAmount    │    │                 │    │ • CompletedAt   │    │                 │
│ • RemainingAmt  │    │                 │    │ • ExpiresAt     │    │                 │
│ • IssueDate     │    │                 │    │ • Metadata      │    │                 │
│ • DueDate       │    │                 │    │                 │    │                 │
│ • FullyPaidDate │    │                 │    │                 │    │                 │
│ • Metadata      │    │                 │    │                 │    │                 │
└─────────────────┘    └─────────────────┘    └─────────────────┘    └─────────────────┘
         │                       │                       │                       │
         │                       │                       │                       │
         │ 1:N                   │ 1:N                   │ 1:N                   │
         │                       │                       │                       │
         ▼                       ▼                       ▼                       ▼
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   BILL ITEM     │    │WALLET TRANSACTION│   │PAYMENT TRANSACTION│   │   (No Entities) │
│   (Entity)      │    │   (Entity)      │    │   (Entity)      │    │                 │
├─────────────────┤    ├─────────────────┤    ├─────────────────┤    ├─────────────────┤
│ • BillId        │    │ • WalletId      │    │ • PaymentId     │    │                 │
│ • Title         │    │ • TransactionType│    │ • TransactionType│    │                 │
│ • Description   │    │ • Amount        │    │ • Amount        │    │                 │
│ • UnitPrice     │    │ • BalanceAfter  │    │ • Status        │    │                 │
│ • Quantity      │    │ • Status        │    │ • GatewayResponse│    │                 │
│ • DiscountPct   │    │ • ReferenceId   │    │ • CreatedAt     │    │                 │
│ • TotalPrice    │    │ • Description   │    │ • Metadata      │    │                 │
│                 │    │ • ExternalRef   │    │                 │    │                 │
│                 │    │ • CreatedAt     │    │                 │    │                 │
│                 │    │ • Metadata      │    │                 │    │                 │
└─────────────────┘    └─────────────────┘    └─────────────────┘    └─────────────────┘

┌─────────────────┐
│ WALLET DEPOSIT  │
│  (Aggregate)    │
├─────────────────┤
│ • WalletId      │
│ • BillId        │
│ • UserNational  │
│ • Amount        │
│ • Status        │
│ • Description   │
│ • ExternalRef   │
│ • RequestedAt   │
│ • CompletedAt   │
│ • Metadata      │
└─────────────────┘
         │
         │ 1:1
         │
         ▼
┌─────────────────┐
│      BILL       │
│  (Referenced)   │
└─────────────────┘
```

## 🔄 Business Flow Relationships

### 1. Wallet Charging Flow
```
User Request → WalletDeposit → Bill → Payment → WalletTransaction
     │              │           │        │            │
     │              │           │        │            │
     ▼              ▼           ▼        ▼            ▼
[Request]    [Pending]    [Issued]  [Completed]  [Balance+]
```

### 2. Bill Payment Flow
```
Bill Creation → BillItem → Payment → PaymentTransaction → Bill Update
     │            │          │            │                │
     │            │          │            │                │
     ▼            ▼          ▼            ▼                ▼
[Draft]      [Items]    [Initiated]   [Processing]     [Paid]
```

### 3. Refund Processing Flow
```
Payment → Refund → WalletTransaction (if applicable)
   │        │              │
   │        │              │
   ▼        ▼              ▼
[Paid]  [Initiated]    [Refund]
```

## 🎭 Domain Events Flow

### Bill Events
```
BillCreated → BillStatusChanged → BillFullyPaid → BillOverdue
     │              │                  │              │
     │              │                  │              │
     ▼              ▼                  ▼              ▼
[Domain]        [Status]            [Payment]      [Timeout]
[Event]         [Change]           [Complete]     [Event]
```

### Wallet Events
```
WalletCreated → WalletBalanceChanged → WalletTransactionCompleted
      │                │                        │
      │                │                        │
      ▼                ▼                        ▼
  [New User]      [Deposit/Withdraw]        [Transaction]
  [Wallet]        [Balance Update]          [Complete]
```

### Payment Events
```
PaymentInitiated → PaymentProcessing → PaymentCompleted
       │                  │                    │
       │                  │                    │
       ▼                  ▼                    ▼
   [Start]            [Gateway]              [Success]
   [Payment]          [Process]             [Complete]
```

## 🏷️ Enum Relationships

### Status Enums
```
BillStatus: Draft → Issued → Paid/Overdue/Cancelled
WalletStatus: Active → Suspended/Closed
PaymentStatus: Initiated → Processing → Completed/Failed/Expired/Cancelled
RefundStatus: Initiated → Processing → Completed/Failed/Cancelled
WalletDepositStatus: Pending → Processing → Completed/Failed/Cancelled/Expired
WalletTransactionStatus: Pending → Processing → Completed/Failed/Cancelled/Refunded
```

### Type Enums
```
BillType: ServiceFee, WalletDeposit, MembershipFee, Penalty
WalletTransactionType: Deposit, Withdrawal, TransferIn, TransferOut, Payment, Refund, Adjustment
PaymentMethod: Card, BankTransfer, Wallet, Cash
PaymentGateway: Parsian, Parbad, Virtual
```

## 🔧 Domain Services

### WalletDomainService
```
┌─────────────────────────────────────────────────────────────┐
│                  WALLET DOMAIN SERVICE                      │
├─────────────────────────────────────────────────────────────┤
│ • AnalyzeBalanceHistory(Wallet, DateRange)                  │
│ • ValidateTransactionLimits(Wallet, Amount, Type)          │
│ • CalculateFees(Amount, TransactionType)                   │
│ • ProcessTransfer(FromWallet, ToWallet, Amount)            │
│ • ValidatePayment(Wallet, Amount, PaymentMethod)           │
└─────────────────────────────────────────────────────────────┘
```

## 🗄️ Repository Interfaces

### Repository Relationships
```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│  IBillRepository│    │IWalletRepository│    │IPaymentRepository│
├─────────────────┤    ├─────────────────┤    ├─────────────────┤
│ • GetByRefId()  │    │ • GetByNational()│    │ • GetByBillId() │
│ • GetByUser()   │    │ • GetActive()   │    │ • GetByStatus() │
│ • GetByStatus() │    │ • GetByStatus() │    │ • GetByGateway()│
│ • GetOverdue()  │    │                 │    │ • GetExpired()  │
└─────────────────┘    └─────────────────┘    └─────────────────┘
         │                       │                       │
         │                       │                       │
         ▼                       ▼                       ▼
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│IWalletTransaction│    │IWalletDeposit   │    │ IRefundRepository│
│   Repository    │    │  Repository     │    │                 │
├─────────────────┤    ├─────────────────┤    ├─────────────────┤
│ • GetByWallet() │    │ • GetByWallet() │    │ • GetByPayment()│
│ • GetByType()   │    │ • GetByStatus() │    │ • GetByStatus() │
│ • GetByDate()   │    │ • GetByExtRef() │    │ • GetByDate()   │
│ • GetBalance()  │    │ • GetPending()  │    │                 │
└─────────────────┘    └─────────────────┘    └─────────────────┘
```

## 🎯 Business Rules Matrix

### Wallet Rules
| Rule | Description | Enforcement |
|------|-------------|-------------|
| Unique Wallet | One wallet per national number | Repository constraint |
| Non-negative Balance | Balance cannot go below zero | Domain validation |
| Transaction Recording | All operations create transactions | Domain method |
| Status Validation | Status affects capabilities | Domain logic |

### Bill Rules
| Rule | Description | Enforcement |
|------|-------------|-------------|
| Multiple Items | Bills can have multiple items | Collection property |
| Partial Payments | Bills support partial payments | Amount tracking |
| Overdue Detection | Bills become overdue after due date | Domain service |
| Cancellation | Bills can be cancelled if not paid | Status validation |

### Payment Rules
| Rule | Description | Enforcement |
|------|-------------|-------------|
| Bill Association | Payments must be linked to bills | Foreign key |
| Gateway Integration | Multiple payment gateways supported | Enum values |
| Status Tracking | Payment status tracked throughout lifecycle | State machine |
| Retry Capability | Failed payments can be retried | Status management |

## 🔐 Security & Integrity

### Data Integrity
- **Foreign Key Constraints**: All relationships enforced at database level
- **Domain Validation**: Business rules enforced in domain layer
- **Immutable Transactions**: Financial transactions cannot be modified
- **Audit Trail**: Complete history of all changes

### Security Measures
- **Access Control**: Repository pattern provides access abstraction
- **Data Encryption**: Sensitive data encrypted at rest and in transit
- **Audit Logging**: All operations logged for compliance
- **Transaction Limits**: Limits enforced at domain level

## 📈 Performance Considerations

### Optimization Strategies
- **Repository Separation**: Different repositories for different entities
- **Query Optimization**: Efficient queries for common operations
- **Indexing Strategy**: Proper database indexing for performance
- **Caching**: Frequently accessed data cached appropriately

### Scalability Design
- **Aggregate Boundaries**: Clear consistency boundaries
- **Event-Driven**: Loose coupling through domain events
- **Repository Pattern**: Clean data access abstraction
- **Domain Services**: Stateless business logic coordination

---

This diagram represents the complete Finance Domain Model with all entities, relationships, business flows, and architectural considerations.
