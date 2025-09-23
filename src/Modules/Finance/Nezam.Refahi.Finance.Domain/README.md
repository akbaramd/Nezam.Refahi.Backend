# 🏦 Finance Domain Context - Nezam Refahi

## 📋 Overview

The Finance Domain Context is a comprehensive financial management system designed for the Nezam Refahi platform. This domain handles all financial operations including billing, payments, refunds, wallet management, and financial transactions within a Domain-Driven Design (DDD) architecture.

## 🎯 Business Purpose

### Primary Objectives
- **Financial Transaction Management**: Complete lifecycle management of financial transactions
- **Billing System**: Comprehensive billing and invoicing capabilities
- **Payment Processing**: Multi-gateway payment processing with transaction tracking
- **Wallet Management**: Digital wallet system for users with balance management
- **Refund Processing**: Automated refund handling and processing
- **Financial Reporting**: Comprehensive financial data and reporting capabilities

### Business Domain Scope
The Finance context manages:
- **Bills & Invoicing**: Creation, management, and payment tracking of bills
- **Payment Processing**: Integration with multiple payment gateways
- **Wallet Operations**: User wallet creation, charging, and transaction management
- **Refund Management**: Automated refund processing and tracking
- **Financial Transactions**: Complete audit trail of all financial activities

## 🏗️ Domain Architecture

### Domain-Driven Design Principles
- **Aggregate Roots**: Bill, Wallet, Payment, Refund
- **Entities**: BillItem, WalletTransaction, WalletDeposit, PaymentTransaction
- **Value Objects**: Money (currency and amount)
- **Domain Events**: Comprehensive event system for state changes
- **Domain Services**: Business logic coordination
- **Repository Pattern**: Clean data access abstraction

## 📊 Domain Models

### 🧾 Core Entities

#### 1. Bill (Aggregate Root)
**Purpose**: Represents a financial document that requires payment
```csharp
public sealed class Bill : FullAggregateRoot<Guid>
```

**Key Properties**:
- `BillNumber`: Unique bill identifier
- `Title`: Bill title/description
- `ReferenceId`: External reference identifier
- `BillType`: Type of bill (e.g., "WalletDeposit", "ServiceFee")
- `UserNationalNumber`: Associated user
- `Status`: Current bill status (Draft, Issued, Paid, Overdue, Cancelled)
- `TotalAmount`: Total bill amount
- `PaidAmount`: Amount paid so far
- `RemainingAmount`: Outstanding amount
- `IssueDate`: When bill was issued
- `DueDate`: Payment due date
- `FullyPaidDate`: When bill was fully paid

**Business Rules**:
- Bills can have multiple items (BillItem)
- Bills support partial payments
- Bills can be cancelled if not paid
- Bills automatically become overdue after due date
- Bills raise domain events for status changes

#### 2. BillItem (Entity)
**Purpose**: Individual line items within a bill
```csharp
public sealed class BillItem : Entity<Guid>
```

**Key Properties**:
- `BillId`: Parent bill reference
- `Title`: Item title
- `Description`: Item description
- `UnitPrice`: Price per unit
- `Quantity`: Number of units
- `DiscountPercentage`: Applied discount
- `TotalPrice`: Calculated total price

#### 3. Wallet (Aggregate Root)
**Purpose**: Digital wallet for user financial management
```csharp
public sealed class Wallet : FullAggregateRoot<Guid>
```

**Key Properties**:
- `NationalNumber`: User's national number
- `Balance`: Current wallet balance
- `Status`: Wallet status (Active, Suspended, Closed)
- `WalletName`: Optional wallet name
- `Description`: Wallet description
- `LastTransactionAt`: Last transaction timestamp
- `Metadata`: Additional metadata

**Business Rules**:
- Each user can have one wallet per national number
- Wallet balance cannot go negative
- All transactions are recorded as WalletTransaction entities
- Wallet status affects transaction capabilities
- Wallet raises events for balance changes

#### 4. WalletTransaction (Entity)
**Purpose**: Individual transaction within a wallet
```csharp
public sealed class WalletTransaction : Entity<Guid>
```

**Key Properties**:
- `WalletId`: Parent wallet reference
- `TransactionType`: Type of transaction (Deposit, Withdrawal, Transfer, etc.)
- `Amount`: Transaction amount
- `BalanceAfter`: Wallet balance after transaction
- `Status`: Transaction status (Pending, Completed, Failed, Cancelled, Refunded)
- `ReferenceId`: External reference
- `Description`: Transaction description
- `ExternalReference`: External system reference
- `CreatedAt`: Transaction timestamp
- `Metadata`: Additional transaction data

#### 5. WalletDeposit (Aggregate Root)
**Purpose**: Deposit request for wallet charging
```csharp
public sealed class WalletDeposit : FullAggregateRoot<Guid>
```

**Key Properties**:
- `WalletId`: Target wallet reference
- `BillId`: Associated bill reference
- `UserNationalNumber`: User's national number
- `Amount`: Deposit amount
- `Status`: Deposit status (Pending, Processing, Completed, Failed, Cancelled, Expired)
- `Description`: Deposit description
- `ExternalReference`: External reference
- `RequestedAt`: Request timestamp
- `CompletedAt`: Completion timestamp
- `Metadata`: Additional metadata

**Business Rules**:
- Deposits are created when users want to charge their wallet
- Deposits are linked to bills for payment processing
- Deposits can be cancelled if not completed
- Completed deposits trigger wallet charging

#### 6. Payment (Aggregate Root)
**Purpose**: Payment processing and tracking
```csharp
public sealed class Payment : FullAggregateRoot<Guid>
```

**Key Properties**:
- `BillId`: Associated bill reference
- `Amount`: Payment amount
- `Status`: Payment status (Initiated, Processing, Completed, Failed, Expired, Cancelled)
- `PaymentMethod`: Method used (Card, BankTransfer, etc.)
- `Gateway`: Payment gateway used
- `GatewayTransactionId`: Gateway transaction reference
- `GatewayReference`: Gateway reference
- `InitiatedAt`: Payment initiation timestamp
- `CompletedAt`: Payment completion timestamp
- `ExpiresAt`: Payment expiration timestamp
- `Metadata`: Additional payment data

#### 7. PaymentTransaction (Entity)
**Purpose**: Individual transaction within a payment
```csharp
public sealed class PaymentTransaction : Entity<Guid>
```

**Key Properties**:
- `PaymentId`: Parent payment reference
- `TransactionType`: Type of transaction
- `Amount`: Transaction amount
- `Status`: Transaction status
- `GatewayResponse`: Gateway response data
- `CreatedAt`: Transaction timestamp
- `Metadata`: Additional transaction data

#### 8. Refund (Aggregate Root)
**Purpose**: Refund processing and management
```csharp
public sealed class Refund : FullAggregateRoot<Guid>
```

**Key Properties**:
- `PaymentId`: Original payment reference
- `Amount`: Refund amount
- `Status`: Refund status (Initiated, Processing, Completed, Failed, Cancelled)
- `Reason`: Refund reason
- `InitiatedAt`: Refund initiation timestamp
- `CompletedAt`: Refund completion timestamp
- `Metadata`: Additional refund data

### 🏷️ Enums

#### BillStatus
- `Draft`: Bill is being created
- `Issued`: Bill is ready for payment
- `Paid`: Bill is fully paid
- `Overdue`: Bill payment is overdue
- `Cancelled`: Bill is cancelled

#### BillType
- `ServiceFee`: Service-related fees
- `WalletDeposit`: Wallet charging bills
- `MembershipFee`: Membership-related fees
- `Penalty`: Penalty charges

#### WalletStatus
- `Active`: Wallet is operational
- `Suspended`: Wallet is temporarily suspended
- `Closed`: Wallet is permanently closed

#### WalletTransactionType
- `Deposit`: Money added to wallet
- `Withdrawal`: Money removed from wallet
- `TransferIn`: Money transferred in
- `TransferOut`: Money transferred out
- `Payment`: Payment made from wallet
- `Refund`: Refund received to wallet
- `Adjustment`: Manual balance adjustment

#### WalletTransactionStatus
- `Pending`: Transaction is pending
- `Processing`: Transaction is being processed
- `Completed`: Transaction is completed
- `Failed`: Transaction failed
- `Cancelled`: Transaction was cancelled
- `Refunded`: Transaction was refunded

#### WalletDepositStatus
- `Pending`: Deposit request is pending payment
- `Processing`: Deposit is processing payment
- `Completed`: Deposit is completed successfully
- `Failed`: Deposit failed
- `Cancelled`: Deposit is cancelled
- `Expired`: Deposit request expired

#### PaymentStatus
- `Initiated`: Payment is initiated
- `Processing`: Payment is being processed
- `Completed`: Payment is completed
- `Failed`: Payment failed
- `Expired`: Payment expired
- `Cancelled`: Payment was cancelled

#### PaymentMethod
- `Card`: Credit/Debit card
- `BankTransfer`: Bank transfer
- `Wallet`: Wallet payment
- `Cash`: Cash payment

#### PaymentGateway
- `Parsian`: Parsian payment gateway
- `Parbad`: Parbad payment gateway
- `Virtual`: Virtual payment gateway

#### RefundStatus
- `Initiated`: Refund is initiated
- `Processing`: Refund is being processed
- `Completed`: Refund is completed
- `Failed`: Refund failed
- `Cancelled`: Refund was cancelled

### 🎭 Domain Events

#### Bill Events
- `BillCreatedEvent`: Bill is created
- `BillStatusChangedEvent`: Bill status changes
- `BillFullyPaidEvent`: Bill is fully paid
- `BillOverdueEvent`: Bill becomes overdue
- `BillCancelledEvent`: Bill is cancelled

#### Wallet Events
- `WalletCreatedEvent`: Wallet is created
- `WalletStatusChangedEvent`: Wallet status changes
- `WalletBalanceChangedEvent`: Wallet balance changes
- `WalletTransactionCompletedEvent`: Wallet transaction completed

#### Wallet Deposit Events
- `WalletDepositRequestedEvent`: Deposit request is created
- `WalletDepositCompletedEvent`: Deposit is completed
- `WalletDepositCancelledEvent`: Deposit is cancelled

#### Payment Events
- `PaymentInitiatedEvent`: Payment is initiated
- `PaymentProcessingEvent`: Payment is processing
- `PaymentCompletedEvent`: Payment is completed
- `PaymentFailedEvent`: Payment failed
- `PaymentExpiredEvent`: Payment expired

#### Refund Events
- `RefundInitiatedEvent`: Refund is initiated
- `RefundCompletedEvent`: Refund is completed
- `RefundFailedEvent`: Refund failed

### 🔧 Domain Services

#### WalletDomainService
**Purpose**: Centralized wallet business logic coordination
```csharp
public sealed class WalletDomainService
```

**Key Methods**:
- `AnalyzeBalanceHistory()`: Analyze wallet balance history
- `ValidateTransactionLimits()`: Validate transaction limits
- `CalculateFees()`: Calculate transaction fees
- `ProcessTransfer()`: Process wallet-to-wallet transfers
- `ValidatePayment()`: Validate payment from wallet

**Business Rules**:
- Stateless service for business logic coordination
- No repository dependencies
- Operates directly on domain entities
- Handles complex business scenarios

### 🗄️ Repository Interfaces

#### IBillRepository
- `GetByReferenceIdAsync()`: Get bill by reference ID
- `GetByUserNationalNumberAsync()`: Get user's bills
- `GetByStatusAsync()`: Get bills by status
- `GetOverdueBillsAsync()`: Get overdue bills

#### IWalletRepository
- `GetByNationalNumberAsync()`: Get wallet by user national number
- `GetActiveWalletsAsync()`: Get active wallets
- `GetWalletsByStatusAsync()`: Get wallets by status

#### IWalletTransactionRepository
- `GetByWalletIdAsync()`: Get wallet transactions
- `GetByTypeAsync()`: Get transactions by type
- `GetByDateRangeAsync()`: Get transactions by date range
- `GetBalanceHistoryAsync()`: Get balance history

#### IWalletDepositRepository
- `GetByWalletIdAsync()`: Get deposits by wallet
- `GetByStatusAsync()`: Get deposits by status
- `GetByExternalReferenceAsync()`: Get deposit by external reference
- `GetPendingDepositsAsync()`: Get pending deposits

#### IPaymentRepository
- `GetByBillIdAsync()`: Get payments by bill
- `GetByStatusAsync()`: Get payments by status
- `GetByGatewayAsync()`: Get payments by gateway
- `GetExpiredPaymentsAsync()`: Get expired payments

#### IRefundRepository
- `GetByPaymentIdAsync()`: Get refunds by payment
- `GetByStatusAsync()`: Get refunds by status
- `GetByDateRangeAsync()`: Get refunds by date range

## 🔄 Business Flows

### 1. Wallet Charging Flow
```
1. User requests wallet charge
2. CreateWalletDepositBillCommand creates:
   - WalletDeposit (Pending status)
   - Bill (WalletDeposit type)
   - Link deposit to bill via BillId
3. User pays the bill
4. BillFullyPaidEvent is raised
5. WalletChargePaymentCompletedConsumer:
   - Finds the deposit
   - Sets deposit to Completed
   - Charges the wallet
   - Creates wallet transaction
```

### 2. Bill Payment Flow
```
1. Bill is created and issued
2. Payment is initiated
3. Payment processing through gateway
4. Payment completion triggers:
   - Bill status update
   - Payment completion events
   - Related business logic
```

### 3. Refund Processing Flow
```
1. Refund request is initiated
2. Refund is processed
3. Original payment is refunded
4. Wallet is credited (if applicable)
5. Refund completion events are raised
```

## 🎯 Key Business Rules

### Wallet Management
- Each user can have only one wallet per national number
- Wallet balance cannot go negative
- All wallet operations must be recorded as transactions
- Wallet status affects transaction capabilities

### Bill Management
- Bills can have multiple items
- Bills support partial payments
- Bills automatically become overdue after due date
- Bills can be cancelled if not paid

### Payment Processing
- Payments are linked to bills
- Payments can be processed through multiple gateways
- Payment status is tracked throughout the lifecycle
- Failed payments can be retried

### Transaction Integrity
- All financial transactions are immutable
- Complete audit trail is maintained
- Transaction status is tracked
- Metadata is preserved for all transactions

## 🔐 Security Considerations

### Data Protection
- Sensitive financial data is encrypted
- Audit trails are maintained
- Access controls are enforced
- Data retention policies are implemented

### Transaction Security
- Transaction validation and verification
- Fraud detection mechanisms
- Secure payment gateway integration
- Transaction limits and controls

## 📈 Scalability & Performance

### Design Considerations
- Repository pattern for data access abstraction
- Domain events for loose coupling
- Aggregate design for consistency boundaries
- Value objects for immutable data

### Performance Optimizations
- Separate repositories for different entities
- Optimized queries for common operations
- Efficient indexing strategies
- Caching for frequently accessed data

## 🧪 Testing Strategy

### Unit Testing
- Domain entity behavior testing
- Business rule validation
- Domain service logic testing
- Repository interface testing

### Integration Testing
- End-to-end business flow testing
- Event handling verification
- Repository implementation testing
- Cross-aggregate interaction testing

## 📚 Dependencies

### Internal Dependencies
- `Nezam.Refahi.Shared.Domain`: Shared domain components
- `MCA.SharedKernel.Domain`: Domain framework components

### External Dependencies
- Entity Framework Core for data persistence
- MediatR for domain event handling
- FluentValidation for input validation

## 🚀 Future Enhancements

### Planned Features
- Multi-currency support
- Advanced reporting capabilities
- Enhanced fraud detection
- Real-time transaction monitoring
- Advanced wallet features (cards, virtual accounts)

### Technical Improvements
- Event sourcing implementation
- CQRS pattern adoption
- Advanced caching strategies
- Microservices architecture migration

## 📞 Support & Maintenance

### Domain Experts
- Finance domain experts
- Business analysts
- Technical architects
- Quality assurance team

### Maintenance Guidelines
- Regular domain model reviews
- Business rule validation
- Performance monitoring
- Security audits

---

**Last Updated**: December 2024  
**Version**: 1.0.0  
**Maintainer**: Nezam Refahi Development Team
