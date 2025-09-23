# 🏦 Finance Application Layer - Nezam Refahi

## 📋 Overview

The Finance Application Layer is the **business logic orchestration layer** of the Finance Domain Context. It implements the **Clean Architecture** principles by coordinating domain operations, handling application-specific business rules, and providing a clean interface between the presentation layer and the domain layer.

## 🎯 Purpose & Responsibilities

### Primary Objectives
- **Business Logic Orchestration**: Coordinate complex business workflows
- **Command & Query Handling**: Process user requests through CQRS pattern
- **Validation & Authorization**: Ensure data integrity and security
- **Event Coordination**: Handle domain events and integration events
- **Transaction Management**: Manage database transactions and consistency
- **External Integration**: Coordinate with payment gateways and external services

### Application Layer Responsibilities
- **Command Processing**: Handle commands that modify system state
- **Query Processing**: Handle queries that retrieve system state
- **Validation**: Validate input data and business rules
- **Authorization**: Ensure users have proper permissions
- **Event Handling**: Process domain events and trigger side effects
- **Integration**: Coordinate with external systems and services

## 🏗️ Architecture Patterns

### Clean Architecture Implementation
```
┌─────────────────────────────────────────────────────────────┐
│                    APPLICATION LAYER                        │
├─────────────────────────────────────────────────────────────┤
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐        │
│  │   Commands  │  │   Queries   │  │   Events    │        │
│  │             │  │             │  │             │        │
│  │ • Create    │  │ • Get       │  │ • Domain    │        │
│  │ • Update    │  │ • List      │  │ • Integration│       │
│  │ • Delete    │  │ • Search    │  │ • Application│       │
│  │ • Process   │  │ • Report    │  │             │        │
│  └─────────────┘  └─────────────┘  └─────────────┘        │
│                                                             │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐        │
│  │  Handlers   │  │ Validators  │  │  Services   │        │
│  │             │  │             │  │             │        │
│  │ • Command   │  │ • Fluent    │  │ • Unit of   │        │
│  │   Handlers  │  │   Validation│  │   Work      │        │
│  │ • Query     │  │ • Business  │  │ • External  │        │
│  │   Handlers  │  │   Rules     │  │   Services  │        │
│  │ • Event     │  │ • Security  │  │ • Integration│       │
│  │   Handlers  │  │   Rules     │  │   Services  │        │
│  └─────────────┘  └─────────────┘  └─────────────┘        │
└─────────────────────────────────────────────────────────────┘
```

### CQRS (Command Query Responsibility Segregation)
The application layer implements CQRS pattern:

- **Commands**: Operations that modify system state
- **Queries**: Operations that retrieve system state
- **Handlers**: Process commands and queries
- **Validators**: Validate input data and business rules

## 📊 Application Features

### 🧾 Bill Management Features

#### Commands
- **CreateBill**: Create new bills with items
- **AddBillItem**: Add items to existing bills
- **RemoveBillItem**: Remove items from bills
- **IssueBill**: Issue bills for payment
- **CancelBill**: Cancel bills that haven't been paid

#### Queries
- **GetUserBills**: Retrieve bills for a specific user
- **GetBillPaymentStatus**: Check payment status of bills

#### Business Logic
```csharp
// CreateBillCommandHandler
public async Task<ApplicationResult<CreateBillResponse>> Handle(
    CreateBillCommand request, CancellationToken cancellationToken)
{
    // 1. Validate input
    var validationResult = await _validator.ValidateAsync(request, cancellationToken);
    if (!validationResult.IsValid)
        return ApplicationResult<CreateBillResponse>.Failure(validationResult.Errors);

    // 2. Check business rules
    if (await _billRepository.ExistsByReferenceIdAsync(request.ReferenceId))
        return ApplicationResult<CreateBillResponse>.Failure("Bill with this reference already exists");

    // 3. Create domain entity
    var bill = new Bill(
        title: request.Title,
        referenceId: request.ReferenceId,
        billType: request.BillType,
        userNationalNumber: request.UserNationalNumber,
        userFullName: request.UserFullName,
        description: request.Description,
        dueDate: request.DueDate,
        metadata: request.Metadata);

    // 4. Add bill items
    foreach (var itemRequest in request.Items)
    {
        bill.AddItem(
            title: itemRequest.Title,
            description: itemRequest.Description,
            unitPrice: Money.FromRials(itemRequest.UnitPriceRials),
            quantity: itemRequest.Quantity,
            discountPercentage: itemRequest.DiscountPercentage);
    }

    // 5. Issue the bill
    bill.Issue();

    // 6. Save to database
    await _billRepository.AddAsync(bill, cancellationToken: cancellationToken);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    // 7. Return response
    return ApplicationResult<CreateBillResponse>.Success(new CreateBillResponse
    {
        BillId = bill.Id,
        BillNumber = bill.BillNumber,
        Status = bill.Status.ToString(),
        TotalAmountRials = bill.TotalAmount.AmountRials,
        IssueDate = bill.IssueDate
    });
}
```

### 💰 Wallet Management Features

#### Commands
- **CreateWallet**: Create new wallets for users
- **ChargeWallet**: Charge wallets with money
- **CreateWalletChargeBill**: Create bills for wallet charging

#### Queries
- **GetWalletBalance**: Retrieve wallet balance and analysis
- **GetWalletTransactions**: Get wallet transaction history
- **GetWalletDeposits**: Get wallet deposit requests

#### Business Logic
```csharp
// ChargeWalletCommandHandler
public async Task<ApplicationResult<ChargeWalletResponse>> Handle(
    ChargeWalletCommand request, CancellationToken cancellationToken)
{
    // 1. Validate input
    var validationResult = await _validator.ValidateAsync(request, cancellationToken);
    if (!validationResult.IsValid)
        return ApplicationResult<ChargeWalletResponse>.Failure(validationResult.Errors);

    // 2. Find wallet
    var wallet = await _walletRepository.GetByNationalNumberAsync(request.UserNationalNumber, cancellationToken);
    if (wallet == null)
        return ApplicationResult<ChargeWalletResponse>.Failure("Wallet not found");

    // 3. Validate wallet status
    if (wallet.Status != WalletStatus.Active)
        return ApplicationResult<ChargeWalletResponse>.Failure($"Wallet is not active. Status: {wallet.Status}");

    // 4. Validate transaction limits
    var amount = Money.FromRials(request.AmountRials);
    var limitValidation = _walletDomainService.ValidateTransactionLimits(wallet, amount, WalletTransactionType.Deposit);
    if (!limitValidation.IsValid)
        return ApplicationResult<ChargeWalletResponse>.Failure($"Transaction limit exceeded: {string.Join(", ", limitValidation.Warnings)}");

    // 5. Charge wallet
    var transaction = wallet.Deposit(
        amount: amount,
        referenceId: request.ReferenceId,
        description: request.Description,
        externalReference: request.ExternalReference,
        metadata: request.Metadata ?? new Dictionary<string, string>());

    // 6. Save changes
    await _walletRepository.UpdateAsync(wallet, cancellationToken: cancellationToken);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    // 7. Return response
    return ApplicationResult<ChargeWalletResponse>.Success(new ChargeWalletResponse
    {
        WalletId = wallet.Id,
        TransactionId = transaction.Id,
        NewBalanceRials = wallet.Balance.AmountRials,
        TransactionType = transaction.TransactionType.ToString(),
        Status = "Completed",
        TransactionDate = transaction.CreatedAt,
        ReferenceId = transaction.ReferenceId ?? string.Empty,
        ExternalReference = transaction.ExternalReference
    });
}
```

### 💳 Payment Processing Features

#### Commands
- **CreatePayment**: Initiate payment processing
- **CompletePayment**: Complete successful payments
- **FailPayment**: Handle failed payments
- **CancelPayment**: Cancel pending payments
- **HandleCallback**: Process payment gateway callbacks

#### Business Logic
```csharp
// CreatePaymentCommandHandler
public async Task<ApplicationResult<CreatePaymentResponse>> Handle(
    CreatePaymentCommand request, CancellationToken cancellationToken)
{
    // 1. Validate input
    var validationResult = await _validator.ValidateAsync(request, cancellationToken);
    if (!validationResult.IsValid)
        return ApplicationResult<CreatePaymentResponse>.Failure(validationResult.Errors);

    // 2. Find bill
    var bill = await _billRepository.GetByIdAsync(request.BillId, cancellationToken);
    if (bill == null)
        return ApplicationResult<CreatePaymentResponse>.Failure("Bill not found");

    // 3. Validate bill status
    if (bill.Status != BillStatus.Issued)
        return ApplicationResult<CreatePaymentResponse>.Failure($"Bill is not issued. Status: {bill.Status}");

    // 4. Check if bill is already paid
    if (bill.RemainingAmount.AmountRials <= 0)
        return ApplicationResult<CreatePaymentResponse>.Failure("Bill is already fully paid");

    // 5. Validate payment amount
    var paymentAmount = Money.FromRials(request.AmountRials);
    if (paymentAmount.AmountRials > bill.RemainingAmount.AmountRials)
        return ApplicationResult<CreatePaymentResponse>.Failure("Payment amount exceeds remaining bill amount");

    // 6. Create payment
    var payment = new Payment(
        billId: bill.Id,
        amount: paymentAmount,
        paymentMethod: request.PaymentMethod,
        gateway: request.Gateway,
        expiresAt: DateTime.UtcNow.AddHours(24));

    // 7. Initiate payment
    payment.Initiate();

    // 8. Save payment
    await _paymentRepository.AddAsync(payment, cancellationToken: cancellationToken);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    // 9. Return response with payment URL
    return ApplicationResult<CreatePaymentResponse>.Success(new CreatePaymentResponse
    {
        PaymentId = payment.Id,
        PaymentUrl = GeneratePaymentUrl(payment),
        ExpiresAt = payment.ExpiresAt,
        Status = payment.Status.ToString()
    });
}
```

### 💸 Refund Processing Features

#### Commands
- **CreateRefund**: Initiate refund processing
- **CompleteRefund**: Complete successful refunds

#### Business Logic
```csharp
// CreateRefundCommandHandler
public async Task<ApplicationResult<CreateRefundResponse>> Handle(
    CreateRefundCommand request, CancellationToken cancellationToken)
{
    // 1. Validate input
    var validationResult = await _validator.ValidateAsync(request, cancellationToken);
    if (!validationResult.IsValid)
        return ApplicationResult<CreateRefundResponse>.Failure(validationResult.Errors);

    // 2. Find payment
    var payment = await _paymentRepository.GetByIdAsync(request.PaymentId, cancellationToken);
    if (payment == null)
        return ApplicationResult<CreateRefundResponse>.Failure("Payment not found");

    // 3. Validate payment status
    if (payment.Status != PaymentStatus.Completed)
        return ApplicationResult<CreateRefundResponse>.Failure($"Payment is not completed. Status: {payment.Status}");

    // 4. Validate refund amount
    var refundAmount = Money.FromRials(request.AmountRials);
    if (refundAmount.AmountRials > payment.Amount.AmountRials)
        return ApplicationResult<CreateRefundResponse>.Failure("Refund amount cannot exceed payment amount");

    // 5. Create refund
    var refund = new Refund(
        paymentId: payment.Id,
        amount: refundAmount,
        reason: request.Reason);

    // 6. Initiate refund
    refund.Initiate();

    // 7. Save refund
    await _refundRepository.AddAsync(refund, cancellationToken: cancellationToken);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    // 8. Return response
    return ApplicationResult<CreateRefundResponse>.Success(new CreateRefundResponse
    {
        RefundId = refund.Id,
        Status = refund.Status.ToString(),
        AmountRials = refund.Amount.AmountRials,
        Reason = refund.Reason
    });
}
```

## 🎭 Event Handling

### Domain Event Processing
The application layer handles domain events through event consumers:

```csharp
// WalletChargePaymentCompletedConsumer
public class WalletChargePaymentCompletedConsumer : INotificationHandler<BillFullyPaidEvent>
{
    public async Task Handle(BillFullyPaidEvent notification, CancellationToken cancellationToken)
    {
        // 1. Check if this is a wallet deposit bill
        if (!IsWalletDepositBill(notification.ReferenceType))
            return;

        // 2. Find the deposit
        if (!Guid.TryParse(notification.ReferenceId, out var depositId))
            return;

        var deposit = await _walletDepositRepository.GetByIdAsync(depositId, cancellationToken);
        if (deposit == null || deposit.Status != WalletDepositStatus.Pending)
            return;

        // 3. Complete the deposit
        deposit.Complete();
        await _walletDepositRepository.UpdateAsync(deposit, cancellationToken: cancellationToken);

        // 4. Charge the wallet
        var chargeCommand = new ChargeWalletCommand
        {
            UserNationalNumber = notification.UserNationalNumber,
            AmountRials = deposit.Amount.AmountRials,
            ReferenceId = deposit.Id.ToString(),
            Description = $"Wallet deposit completion via bill payment - DepositId: {depositId}"
        };

        var result = await _mediator.Send(chargeCommand, cancellationToken);
        
        // 5. Log results
        if (result.IsSuccess)
        {
            _logger.LogInformation("Successfully completed deposit {DepositId} and charged wallet", depositId);
        }
        else
        {
            _logger.LogError("Failed to charge wallet after completing deposit {DepositId}", depositId);
        }
    }
}
```

## 🔧 Application Services

### Unit of Work Pattern
```csharp
public interface IFinanceUnitOfWork : IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}

public class FinanceUnitOfWork : BaseUnitOfWork<FinanceDbContext>, IFinanceUnitOfWork
{
    public FinanceUnitOfWork(FinanceDbContext context) : base(context)
    {
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await Context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        await Context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        await Context.Database.CommitTransactionAsync(cancellationToken);
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        await Context.Database.RollbackTransactionAsync(cancellationToken);
    }
}
```

## ✅ Validation Framework

### FluentValidation Implementation
All commands and queries are validated using FluentValidation:

```csharp
// CreateWalletCommandValidator
public class CreateWalletCommandValidator : AbstractValidator<CreateWalletCommand>
{
    public CreateWalletCommandValidator()
    {
        RuleFor(x => x.UserNationalNumber)
            .NotEmpty()
            .WithMessage("User national number is required")
            .MaximumLength(20)
            .WithMessage("User national number cannot exceed 20 characters")
            .Matches(@"^\d{10}$")
            .WithMessage("User national number must be exactly 10 digits");

        RuleFor(x => x.UserFullName)
            .NotEmpty()
            .WithMessage("User full name is required")
            .MaximumLength(100)
            .WithMessage("User full name cannot exceed 100 characters");

        RuleFor(x => x.InitialBalanceRials)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Initial balance cannot be negative");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Description cannot exceed 500 characters");
    }
}

// GetWalletBalanceQueryValidator
public class GetWalletBalanceQueryValidator : AbstractValidator<GetWalletBalanceQuery>
{
    public GetWalletBalanceQueryValidator()
    {
        RuleFor(x => x.UserNationalNumber)
            .NotEmpty()
            .WithMessage("User national number is required")
            .MaximumLength(20)
            .WithMessage("User national number cannot exceed 20 characters");
    }
}
```

## 🔐 Security & Authorization

### Input Validation
- **Data Type Validation**: Ensure correct data types
- **Range Validation**: Validate numeric ranges
- **Format Validation**: Validate string formats (national numbers, etc.)
- **Business Rule Validation**: Validate business-specific rules

### Authorization Checks
```csharp
// Example authorization in handlers
public async Task<ApplicationResult<WalletBalanceResponse>> Handle(
    GetWalletBalanceQuery request, CancellationToken cancellationToken)
{
    // 1. Validate input
    var validationResult = await _validator.ValidateAsync(request, cancellationToken);
    if (!validationResult.IsValid)
        return ApplicationResult<WalletBalanceResponse>.Failure(validationResult.Errors);

    // 2. Authorization check (if needed)
    // if (!await _authorizationService.CanAccessWallet(request.UserNationalNumber))
    //     return ApplicationResult<WalletBalanceResponse>.Failure("Access denied");

    // 3. Business logic
    var wallet = await _walletRepository.GetByNationalNumberAsync(request.UserNationalNumber, cancellationToken);
    if (wallet == null)
        return ApplicationResult<WalletBalanceResponse>.Failure("Wallet not found");

    // 4. Return response
    return ApplicationResult<WalletBalanceResponse>.Success(response);
}
```

## 📊 Error Handling

### Application Result Pattern
```csharp
public class ApplicationResult<T>
{
    public bool IsSuccess { get; }
    public T? Data { get; }
    public string[] Errors { get; }

    private ApplicationResult(bool isSuccess, T? data, string[] errors)
    {
        IsSuccess = isSuccess;
        Data = data;
        Errors = errors;
    }

    public static ApplicationResult<T> Success(T data)
    {
        return new ApplicationResult<T>(true, data, Array.Empty<string>());
    }

    public static ApplicationResult<T> Failure(params string[] errors)
    {
        return new ApplicationResult<T>(false, default, errors);
    }
}
```

### Exception Handling
```csharp
public async Task<ApplicationResult<CreateWalletResponse>> Handle(
    CreateWalletCommand request, CancellationToken cancellationToken)
{
    try
    {
        // Business logic
        return ApplicationResult<CreateWalletResponse>.Success(response);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error creating wallet for user {UserNationalNumber}", request.UserNationalNumber);
        return ApplicationResult<CreateWalletResponse>.Failure(
            $"An error occurred while creating wallet: {ex.Message}");
    }
}
```

## 🔄 Transaction Management

### Database Transactions
```csharp
public async Task<ApplicationResult<ChargeWalletResponse>> Handle(
    ChargeWalletCommand request, CancellationToken cancellationToken)
{
    using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);
    try
    {
        // 1. Update wallet
        var transaction = wallet.Deposit(amount, referenceId, description);
        await _walletRepository.UpdateAsync(wallet, cancellationToken: cancellationToken);

        // 2. Create transaction record
        await _walletTransactionRepository.AddAsync(transaction, cancellationToken: cancellationToken);

        // 3. Commit transaction
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _unitOfWork.CommitTransactionAsync(cancellationToken);

        return ApplicationResult<ChargeWalletResponse>.Success(response);
    }
    catch (Exception ex)
    {
        await _unitOfWork.RollbackTransactionAsync(cancellationToken);
        throw;
    }
}
```

## 📈 Performance Considerations

### Query Optimization
```csharp
// Optimized query with specific includes
public async Task<Wallet?> GetWithRecentTransactionsAsync(Guid walletId, CancellationToken cancellationToken = default)
{
    return await Context.Wallets
        .Include(w => w.Transactions.OrderByDescending(t => t.CreatedAt).Take(10))
        .FirstOrDefaultAsync(w => w.Id == walletId, cancellationToken);
}

// Paginated queries
public async Task<IEnumerable<WalletTransaction>> GetTransactionsAsync(
    Guid walletId, int page, int pageSize, CancellationToken cancellationToken = default)
{
    return await Context.WalletTransactions
        .Where(t => t.WalletId == walletId)
        .OrderByDescending(t => t.CreatedAt)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync(cancellationToken);
}
```

### Caching Strategy
```csharp
// Cache frequently accessed data
public async Task<Wallet?> GetByNationalNumberAsync(string nationalNumber, CancellationToken cancellationToken = default)
{
    var cacheKey = $"wallet:{nationalNumber}";
    
    if (_cache.TryGetValue(cacheKey, out Wallet? cachedWallet))
        return cachedWallet;

    var wallet = await Context.Wallets
        .FirstOrDefaultAsync(w => w.NationalNumber == nationalNumber, cancellationToken);

    if (wallet != null)
    {
        _cache.Set(cacheKey, wallet, TimeSpan.FromMinutes(5));
    }

    return wallet;
}
```

## 🧪 Testing Strategy

### Unit Testing
```csharp
[Test]
public async Task CreateWallet_ShouldReturnSuccess_WhenValidInput()
{
    // Arrange
    var command = new CreateWalletCommand
    {
        UserNationalNumber = "1234567890",
        UserFullName = "Test User",
        InitialBalanceRials = 0
    };

    var handler = new CreateWalletCommandHandler(
        _mockWalletRepository.Object,
        _mockValidator.Object,
        _mockUnitOfWork.Object,
        _mockWalletDomainService.Object);

    // Act
    var result = await handler.Handle(command, CancellationToken.None);

    // Assert
    Assert.That(result.IsSuccess, Is.True);
    Assert.That(result.Data, Is.Not.Null);
    Assert.That(result.Data.WalletId, Is.Not.EqualTo(Guid.Empty));
}
```

### Integration Testing
```csharp
[Test]
public async Task WalletChargingFlow_ShouldCompleteSuccessfully()
{
    // Arrange
    var userNationalNumber = "1234567890";
    var wallet = new Wallet(userNationalNumber, Money.Zero);
    var depositAmount = Money.FromRials(100000);

    // Act
    var createDepositCommand = new CreateBillChargeBillCommand
    {
        UserNationalNumber = userNationalNumber,
        UserFullName = "Test User",
        AmountRials = depositAmount.AmountRials
    };

    var createResult = await _mediator.Send(createDepositCommand);
    Assert.That(createResult.IsSuccess, Is.True);

    // Simulate payment completion
    var bill = await _billRepository.GetByIdAsync(createResult.Data!.BillId);
    bill.MarkAsPaid(depositAmount);
    await _billRepository.UpdateAsync(bill);
    await _unitOfWork.SaveChangesAsync();

    // Assert wallet is charged
    var updatedWallet = await _walletRepository.GetByNationalNumberAsync(userNationalNumber);
    Assert.That(updatedWallet!.Balance.AmountRials, Is.EqualTo(depositAmount.AmountRials));
}
```

## 📚 Dependencies

### Internal Dependencies
- `Nezam.Refahi.Finance.Domain`: Domain entities and business logic
- `Nezam.Refahi.Finance.Contracts`: Command and query contracts
- `Nezam.Refahi.Shared.Application`: Shared application components

### External Dependencies
- **MediatR**: Command and query handling
- **FluentValidation**: Input validation
- **Entity Framework Core**: Data access
- **Microsoft.Extensions.Logging**: Logging
- **Microsoft.Extensions.DependencyInjection**: Dependency injection

## 🚀 Configuration

### Application Module Configuration
```csharp
public class NezamRefahiFinanceApplicationModule : BonModule
{
    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // Add MediatR
        context.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        // Add FluentValidation
        context.Services.AddValidatorsFromAssembly(assembly);

        // Register configuration
        var configuration = context.GetRequireService<IConfiguration>();
        context.Services.Configure<FrontendSettings>(configuration.GetSection(FrontendSettings.SectionName));

        return base.OnConfigureAsync(context);
    }
}
```

### Frontend Settings
```csharp
public class FrontendSettings
{
    public const string SectionName = "Frontend";
    
    public string BaseUrl { get; set; } = string.Empty;
    public string PaymentCallbackUrl { get; set; } = string.Empty;
    public string RefundCallbackUrl { get; set; } = string.Empty;
}
```

## 📊 Monitoring & Logging

### Structured Logging
```csharp
public async Task<ApplicationResult<CreateWalletResponse>> Handle(
    CreateWalletCommand request, CancellationToken cancellationToken)
{
    _logger.LogInformation("Creating wallet for user {UserNationalNumber}", request.UserNationalNumber);
    
    try
    {
        // Business logic
        _logger.LogInformation("Successfully created wallet {WalletId} for user {UserNationalNumber}", 
            wallet.Id, request.UserNationalNumber);
        
        return ApplicationResult<CreateWalletResponse>.Success(response);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Failed to create wallet for user {UserNationalNumber}", request.UserNationalNumber);
        return ApplicationResult<CreateWalletResponse>.Failure("Failed to create wallet");
    }
}
```

### Performance Monitoring
```csharp
public async Task<ApplicationResult<WalletBalanceResponse>> Handle(
    GetWalletBalanceQuery request, CancellationToken cancellationToken)
{
    using var activity = _activitySource.StartActivity("GetWalletBalance");
    activity?.SetTag("user.national_number", request.UserNationalNumber);
    
    var stopwatch = Stopwatch.StartNew();
    
    try
    {
        // Business logic
        activity?.SetTag("wallet.balance", wallet.Balance.AmountRials);
        activity?.SetTag("operation.duration_ms", stopwatch.ElapsedMilliseconds);
        
        return ApplicationResult<WalletBalanceResponse>.Success(response);
    }
    finally
    {
        stopwatch.Stop();
        _logger.LogInformation("GetWalletBalance completed in {Duration}ms for user {UserNationalNumber}", 
            stopwatch.ElapsedMilliseconds, request.UserNationalNumber);
    }
}
```

## 🔧 Best Practices

### 1. Command & Query Design
- **Single Responsibility**: Each command/query has one purpose
- **Immutable**: Commands and queries are immutable
- **Validation**: All inputs are validated
- **Error Handling**: Proper error handling and logging

### 2. Handler Implementation
- **Dependency Injection**: Use constructor injection
- **Async/Await**: Use async patterns consistently
- **Transaction Management**: Proper transaction handling
- **Logging**: Comprehensive logging for debugging

### 3. Validation Strategy
- **Input Validation**: Validate all inputs
- **Business Rules**: Enforce business rules
- **Security**: Validate security constraints
- **Performance**: Optimize validation performance

### 4. Error Handling
- **Graceful Degradation**: Handle errors gracefully
- **User-Friendly Messages**: Provide clear error messages
- **Logging**: Log errors for debugging
- **Monitoring**: Monitor error rates

### 5. Performance Optimization
- **Query Optimization**: Optimize database queries
- **Caching**: Cache frequently accessed data
- **Pagination**: Use pagination for large datasets
- **Async Operations**: Use async patterns

---

**Last Updated**: December 2024  
**Version**: 1.0.0  
**Maintainer**: Nezam Refahi Development Team

This application layer provides a comprehensive, well-structured business logic orchestration system that follows Clean Architecture principles and implements best practices for maintainable, scalable, and secure financial operations.
