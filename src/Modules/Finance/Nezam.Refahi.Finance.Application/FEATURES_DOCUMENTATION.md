# 🎯 Finance Application Features Documentation

## 📋 Overview

This document provides detailed documentation of all features implemented in the Finance Application layer, including commands, queries, business logic, and integration patterns.

## 🏗️ Feature Architecture

### Feature Organization
```
Features/
├── Bills/
│   ├── Commands/
│   │   ├── CreateBill/
│   │   ├── AddBillItem/
│   │   ├── RemoveBillItem/
│   │   ├── IssueBill/
│   │   └── CancelBill/
│   └── Queries/
│       ├── GetUserBills/
│       └── GetBillPaymentStatus/
├── Payments/
│   ├── Commands/
│   │   ├── CreatePayment/
│   │   ├── CompletePayment/
│   │   ├── FailPayment/
│   │   ├── CancelPayment/
│   │   └── HandleCallback/
├── Refunds/
│   ├── Commands/
│   │   ├── CreateRefund/
│   │   └── CompleteRefund/
└── Wallets/
    ├── Commands/
    │   ├── CreateWallet/
    │   ├── ChargeWallet/
    │   └── CreateWalletChargeBill/
    └── Queries/
        ├── GetWalletBalance/
        ├── GetWalletTransactions/
        └── GetWalletDeposits/
```

## 🧾 Bill Management Features

### 📝 CreateBill Feature

#### Purpose
Creates new bills with items for users to pay.

#### Command Structure
```csharp
public record CreateBillCommand : IRequest<ApplicationResult<CreateBillResponse>>
{
    public string Title { get; init; } = string.Empty;
    public string ReferenceId { get; init; } = string.Empty;
    public string BillType { get; init; } = string.Empty;
    public string UserNationalNumber { get; init; } = string.Empty;
    public string UserFullName { get; init; } = string.Empty;
    public string? Description { get; init; }
    public DateTime? DueDate { get; init; }
    public List<CreateBillItemRequest> Items { get; init; } = new();
    public Dictionary<string, string>? Metadata { get; init; }
}

public record CreateBillItemRequest
{
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public long UnitPriceRials { get; init; }
    public int Quantity { get; init; }
    public decimal DiscountPercentage { get; init; }
}
```

#### Business Logic
1. **Input Validation**: Validate all required fields
2. **Reference Check**: Ensure reference ID is unique
3. **Bill Creation**: Create bill with draft status
4. **Item Addition**: Add all bill items
5. **Bill Issuance**: Issue bill for payment
6. **Persistence**: Save to database
7. **Response**: Return bill details

#### Handler Implementation
```csharp
public class CreateBillCommandHandler : IRequestHandler<CreateBillCommand, ApplicationResult<CreateBillResponse>>
{
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
}
```

#### Validation Rules
```csharp
public class CreateBillCommandValidator : AbstractValidator<CreateBillCommand>
{
    public CreateBillCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Bill title is required")
            .MaximumLength(200)
            .WithMessage("Bill title cannot exceed 200 characters");

        RuleFor(x => x.ReferenceId)
            .NotEmpty()
            .WithMessage("Reference ID is required")
            .MaximumLength(100)
            .WithMessage("Reference ID cannot exceed 100 characters");

        RuleFor(x => x.BillType)
            .NotEmpty()
            .WithMessage("Bill type is required")
            .MaximumLength(50)
            .WithMessage("Bill type cannot exceed 50 characters");

        RuleFor(x => x.UserNationalNumber)
            .NotEmpty()
            .WithMessage("User national number is required")
            .Matches(@"^\d{10}$")
            .WithMessage("User national number must be exactly 10 digits");

        RuleFor(x => x.UserFullName)
            .NotEmpty()
            .WithMessage("User full name is required")
            .MaximumLength(100)
            .WithMessage("User full name cannot exceed 100 characters");

        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("Bill must have at least one item");

        RuleForEach(x => x.Items)
            .SetValidator(new CreateBillItemRequestValidator());
    }
}
```

### 📋 GetUserBills Feature

#### Purpose
Retrieves bills for a specific user with filtering and pagination.

#### Query Structure
```csharp
public record GetUserBillsQuery : IRequest<ApplicationResult<UserBillsResponse>>
{
    public string UserNationalNumber { get; init; } = string.Empty;
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? Status { get; init; }
    public string? BillType { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
}
```

#### Business Logic
1. **Input Validation**: Validate user national number and pagination
2. **Query Building**: Build filtered query
3. **Pagination**: Apply pagination
4. **Data Retrieval**: Fetch bills from repository
5. **Response Mapping**: Map to response DTOs

#### Handler Implementation
```csharp
public class GetUserBillsQueryHandler : IRequestHandler<GetUserBillsQuery, ApplicationResult<UserBillsResponse>>
{
    public async Task<ApplicationResult<UserBillsResponse>> Handle(
        GetUserBillsQuery request, CancellationToken cancellationToken)
    {
        // 1. Validate input
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return ApplicationResult<UserBillsResponse>.Failure(validationResult.Errors);

        // 2. Build query
        var query = _billRepository.GetAll()
            .Where(b => b.UserNationalNumber == request.UserNationalNumber);

        // 3. Apply filters
        if (!string.IsNullOrEmpty(request.Status))
        {
            if (Enum.TryParse<BillStatus>(request.Status, true, out var status))
            {
                query = query.Where(b => b.Status == status);
            }
        }

        if (!string.IsNullOrEmpty(request.BillType))
        {
            query = query.Where(b => b.BillType == request.BillType);
        }

        if (request.FromDate.HasValue)
        {
            query = query.Where(b => b.IssueDate >= request.FromDate.Value);
        }

        if (request.ToDate.HasValue)
        {
            query = query.Where(b => b.IssueDate <= request.ToDate.Value);
        }

        // 4. Get total count
        var totalCount = await query.CountAsync(cancellationToken);

        // 5. Apply pagination
        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);
        var skip = (request.PageNumber - 1) * request.PageSize;

        var bills = await query
            .OrderByDescending(b => b.IssueDate)
            .Skip(skip)
            .Take(request.PageSize)
            .Select(b => new BillSummaryDto
            {
                BillId = b.Id,
                BillNumber = b.BillNumber,
                Title = b.Title,
                BillType = b.BillType,
                Status = b.Status.ToString(),
                TotalAmountRials = b.TotalAmount.AmountRials,
                PaidAmountRials = b.PaidAmount.AmountRials,
                RemainingAmountRials = b.RemainingAmount.AmountRials,
                IssueDate = b.IssueDate,
                DueDate = b.DueDate,
                FullyPaidDate = b.FullyPaidDate
            })
            .ToListAsync(cancellationToken);

        // 6. Return response
        return ApplicationResult<UserBillsResponse>.Success(new UserBillsResponse
        {
            UserNationalNumber = request.UserNationalNumber,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalPages = totalPages,
            Bills = bills
        });
    }
}
```

## 💰 Wallet Management Features

### 🏦 CreateWallet Feature

#### Purpose
Creates a new wallet for a user with optional initial balance.

#### Command Structure
```csharp
public record CreateWalletCommand : IRequest<ApplicationResult<CreateWalletResponse>>
{
    public string UserNationalNumber { get; init; } = string.Empty;
    public string UserFullName { get; init; } = string.Empty;
    public long InitialBalanceRials { get; init; } = 0;
    public string? WalletName { get; init; }
    public string? Description { get; init; }
    public Dictionary<string, string>? Metadata { get; init; }
}
```

#### Business Logic
1. **Input Validation**: Validate user data and initial balance
2. **Duplicate Check**: Ensure user doesn't already have a wallet
3. **Wallet Creation**: Create wallet with initial balance
4. **Transaction Recording**: Record initial balance transaction
5. **Persistence**: Save wallet to database
6. **Response**: Return wallet details

#### Handler Implementation
```csharp
public class CreateWalletCommandHandler : IRequestHandler<CreateWalletCommand, ApplicationResult<CreateWalletResponse>>
{
    public async Task<ApplicationResult<CreateWalletResponse>> Handle(
        CreateWalletCommand request, CancellationToken cancellationToken)
    {
        // 1. Validate input
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return ApplicationResult<CreateWalletResponse>.Failure(validationResult.Errors);

        // 2. Check if wallet already exists
        if (await _walletRepository.ExistsByNationalNumberAsync(request.UserNationalNumber, cancellationToken))
            return ApplicationResult<CreateWalletResponse>.Failure("Wallet already exists for this user");

        // 3. Create wallet
        var initialBalance = Money.FromRials(request.InitialBalanceRials);
        var wallet = new Wallet(
            nationalNumber: request.UserNationalNumber,
            initialBalance: initialBalance,
            walletName: request.WalletName,
            description: request.Description,
            metadata: request.Metadata);

        // 4. Save wallet
        await _walletRepository.AddAsync(wallet, cancellationToken: cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 5. Return response
        return ApplicationResult<CreateWalletResponse>.Success(new CreateWalletResponse
        {
            WalletId = wallet.Id,
            UserNationalNumber = wallet.NationalNumber,
            UserFullName = wallet.WalletName ?? request.UserFullName,
            InitialBalanceRials = wallet.Balance.AmountRials,
            Status = wallet.Status.ToString(),
            CreatedAt = wallet.CreatedAt
        });
    }
}
```

### 💳 ChargeWallet Feature

#### Purpose
Charges a wallet with money through direct deposit.

#### Command Structure
```csharp
public record ChargeWalletCommand : IRequest<ApplicationResult<ChargeWalletResponse>>
{
    public string UserNationalNumber { get; init; } = string.Empty;
    public long AmountRials { get; init; }
    public string? ReferenceId { get; init; }
    public string? Description { get; init; }
    public string? ExternalReference { get; init; }
    public Dictionary<string, string>? Metadata { get; init; }
}
```

#### Business Logic
1. **Input Validation**: Validate amount and user data
2. **Wallet Lookup**: Find user's wallet
3. **Status Check**: Ensure wallet is active
4. **Limit Validation**: Check transaction limits
5. **Wallet Charging**: Deposit money to wallet
6. **Transaction Recording**: Record the transaction
7. **Persistence**: Save changes to database
8. **Response**: Return transaction details

#### Handler Implementation
```csharp
public class ChargeWalletCommandHandler : IRequestHandler<ChargeWalletCommand, ApplicationResult<ChargeWalletResponse>>
{
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
}
```

### 🏦 CreateWalletChargeBill Feature

#### Purpose
Creates a bill for wallet charging that users can pay to charge their wallet.

#### Command Structure
```csharp
public record CreateBillChargeBillCommand : IRequest<ApplicationResult<CreateBillChargeBillResponse>>
{
    public string UserNationalNumber { get; init; } = string.Empty;
    public string UserFullName { get; init; } = string.Empty;
    public decimal AmountRials { get; init; }
    public string? Description { get; init; }
    public Dictionary<string, string>? Metadata { get; init; }
}
```

#### Business Logic
1. **Input Validation**: Validate user data and amount
2. **Wallet Check**: Ensure user has a wallet
3. **Wallet Status**: Check wallet is active
4. **Deposit Creation**: Create wallet deposit request
5. **Bill Creation**: Create bill for payment
6. **Deposit Linking**: Link deposit to bill
7. **Bill Issuance**: Issue bill for payment
8. **Persistence**: Save both entities
9. **Response**: Return bill and deposit details

#### Handler Implementation
```csharp
public class CreateWalletChargeBillCommandHandler : IRequestHandler<CreateBillChargeBillCommand, ApplicationResult<CreateBillChargeBillResponse>>
{
    public async Task<ApplicationResult<CreateBillChargeBillResponse>> Handle(
        CreateBillChargeBillCommand request, CancellationToken cancellationToken)
    {
        // 1. Validate input
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return ApplicationResult<CreateBillChargeBillResponse>.Failure(validationResult.Errors);

        // 2. Check if wallet exists
        var wallet = await _walletRepository.GetByNationalNumberAsync(request.UserNationalNumber, cancellationToken);
        if (wallet == null)
            return ApplicationResult<CreateBillChargeBillResponse>.Failure("Wallet not found for this user");

        // 3. Check wallet status
        if (wallet.Status != WalletStatus.Active)
            return ApplicationResult<CreateBillChargeBillResponse>.Failure($"Cannot create charge bill. Wallet status: {wallet.Status}");

        // 4. Create wallet deposit
        var chargeAmount = Money.FromRials(request.AmountRials);
        var walletDeposit = new WalletDeposit(
            walletId: wallet.Id,
            userNationalNumber: request.UserNationalNumber,
            amount: chargeAmount,
            description: request.Description ?? "Wallet charging deposit",
            externalReference: null,
            metadata: request.Metadata);

        await _walletDepositRepository.AddAsync(walletDeposit, cancellationToken: cancellationToken);

        // 5. Create bill
        var referenceId = walletDeposit.Id.ToString();
        var bill = new Bill(
            title: $"Wallet Deposit - {request.UserFullName}",
            referenceId: referenceId,
            billType: "WalletDeposit",
            userNationalNumber: request.UserNationalNumber,
            userFullName: request.UserFullName,
            description: request.Description ?? "Wallet depositing bill",
            dueDate: DateTime.UtcNow.AddDays(7),
            metadata: request.Metadata);

        // 6. Add bill item
        bill.AddItem(
            title: "Wallet Deposit Amount",
            description: $"Deposit wallet with {request.AmountRials:N0} rials",
            unitPrice: chargeAmount,
            quantity: 1,
            discountPercentage: 0);

        // 7. Issue bill
        bill.Issue();

        // 8. Save bill
        await _billRepository.AddAsync(bill, cancellationToken: cancellationToken);

        // 9. Link deposit to bill
        walletDeposit.SetBillId(bill.Id);
        await _walletDepositRepository.UpdateAsync(walletDeposit, cancellationToken: cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 10. Return response
        return ApplicationResult<CreateBillChargeBillResponse>.Success(new CreateBillChargeBillResponse
        {
            BillId = bill.Id,
            BillNumber = bill.BillNumber,
            UserNationalNumber = request.UserNationalNumber,
            UserFullName = request.UserFullName,
            AmountRials = request.AmountRials,
            Status = bill.Status.ToString(),
            IssueDate = bill.IssueDate,
            ReferenceId = referenceId,
            DepositId = walletDeposit.Id
        });
    }
}
```

### 📊 GetWalletBalance Feature

#### Purpose
Retrieves comprehensive wallet balance information including recent transactions and analysis.

#### Query Structure
```csharp
public record GetWalletBalanceQuery : IRequest<ApplicationResult<WalletBalanceResponse>>
{
    public string UserNationalNumber { get; init; } = string.Empty;
}
```

#### Business Logic
1. **Input Validation**: Validate user national number
2. **Wallet Lookup**: Find user's wallet
3. **Transaction Retrieval**: Get recent transactions
4. **Balance Analysis**: Analyze balance history
5. **Response Mapping**: Map to response DTO

#### Handler Implementation
```csharp
public class GetWalletBalanceQueryHandler : IRequestHandler<GetWalletBalanceQuery, ApplicationResult<WalletBalanceResponse>>
{
    public async Task<ApplicationResult<WalletBalanceResponse>> Handle(
        GetWalletBalanceQuery request, CancellationToken cancellationToken)
    {
        // 1. Validate input
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return ApplicationResult<WalletBalanceResponse>.Failure(validationResult.Errors);

        // 2. Find wallet
        var wallet = await _walletRepository.GetByNationalNumberAsync(request.UserNationalNumber, cancellationToken);
        if (wallet == null)
            return ApplicationResult<WalletBalanceResponse>.Failure("Wallet not found");

        // 3. Get recent transactions
        var recentTransactions = await _walletTransactionRepository.GetByWalletIdAsync(wallet.Id, cancellationToken);
        var last10Transactions = recentTransactions
            .OrderByDescending(t => t.CreatedAt)
            .Take(10)
            .ToList();

        // 4. Analyze balance history
        var analysis = _walletDomainService.AnalyzeBalanceHistory(wallet);

        // 5. Get last transaction
        var lastTransaction = recentTransactions
            .OrderByDescending(t => t.CreatedAt)
            .FirstOrDefault();

        // 6. Return response
        return ApplicationResult<WalletBalanceResponse>.Success(new WalletBalanceResponse
        {
            WalletId = wallet.Id,
            UserNationalNumber = wallet.NationalNumber,
            UserFullName = wallet.WalletName ?? string.Empty,
            CurrentBalanceRials = wallet.Balance.AmountRials,
            Status = wallet.Status.ToString(),
            LastTransactionAt = wallet.LastTransactionAt,
            CreatedAt = wallet.CreatedAt,
            Transactions = last10Transactions.Select(t => new WalletTransactionSummaryDto
            {
                TransactionId = t.Id,
                TransactionType = t.TransactionType.ToString(),
                AmountRials = t.Amount.AmountRials,
                BalanceAfterRials = t.BalanceAfter.AmountRials,
                Status = "Completed",
                TransactionDate = t.CreatedAt,
                ReferenceId = t.ReferenceId ?? string.Empty,
                Description = t.Description ?? string.Empty
            }).ToList(),
            LastTransaction = lastTransaction != null ? new WalletTransactionSummaryDto
            {
                TransactionId = lastTransaction.Id,
                TransactionType = lastTransaction.TransactionType.ToString(),
                AmountRials = lastTransaction.Amount.AmountRials,
                BalanceAfterRials = lastTransaction.BalanceAfter.AmountRials,
                Status = "Completed",
                TransactionDate = lastTransaction.CreatedAt,
                ReferenceId = lastTransaction.ReferenceId ?? string.Empty,
                Description = lastTransaction.Description ?? string.Empty
            } : null,
            Analysis = new WalletBalanceAnalysisDto
            {
                TotalDeposits = analysis.TotalDeposits,
                TotalWithdrawals = analysis.TotalWithdrawals,
                NetChange = analysis.NetChange,
                TotalTransactions = analysis.TransactionCount,
                AverageTransactionAmount = analysis.AverageTransactionAmount,
                AnalysisPeriod = new DateRangeDto
                {
                    FromDate = analysis.AnalysisPeriod.FromDate,
                    ToDate = analysis.AnalysisPeriod.ToDate
                }
            }
        });
    }
}
```

## 💳 Payment Processing Features

### 💰 CreatePayment Feature

#### Purpose
Initiates payment processing for a bill through payment gateways.

#### Command Structure
```csharp
public record CreatePaymentCommand : IRequest<ApplicationResult<CreatePaymentResponse>>
{
    public Guid BillId { get; init; }
    public long AmountRials { get; init; }
    public PaymentMethod PaymentMethod { get; init; }
    public PaymentGateway Gateway { get; init; }
    public Dictionary<string, string>? Metadata { get; init; }
}
```

#### Business Logic
1. **Input Validation**: Validate payment data
2. **Bill Lookup**: Find the bill to pay
3. **Bill Status Check**: Ensure bill is issued
4. **Payment Amount Validation**: Check payment amount
5. **Payment Creation**: Create payment entity
6. **Payment Initiation**: Initiate payment processing
7. **Persistence**: Save payment to database
8. **Response**: Return payment details and URL

#### Handler Implementation
```csharp
public class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, ApplicationResult<CreatePaymentResponse>>
{
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

        // 9. Return response
        return ApplicationResult<CreatePaymentResponse>.Success(new CreatePaymentResponse
        {
            PaymentId = payment.Id,
            PaymentUrl = GeneratePaymentUrl(payment),
            ExpiresAt = payment.ExpiresAt,
            Status = payment.Status.ToString()
        });
    }
}
```

## 🔄 Event Handling Features

### 📨 WalletChargePaymentCompletedConsumer

#### Purpose
Handles the completion of wallet charge bill payments and automatically charges the wallet.

#### Event Structure
```csharp
public class BillFullyPaidEvent : DomainEvent
{
    public Guid BillId { get; }
    public string BillNumber { get; }
    public string ReferenceId { get; }
    public string ReferenceType { get; }
    public string UserNationalNumber { get; }
    public Money TotalAmount { get; }
    public Money PaidAmount { get; }
    public DateTime FullyPaidDate { get; }
    public int PaymentCount { get; }
    public string? LastPaymentMethod { get; }
    public string? LastGateway { get; }
}
```

#### Business Logic
1. **Event Validation**: Check if this is a wallet deposit bill
2. **Deposit Lookup**: Find the wallet deposit
3. **Status Validation**: Ensure deposit is pending
4. **Deposit Completion**: Mark deposit as completed
5. **Wallet Charging**: Charge the wallet
6. **Transaction Recording**: Record wallet transaction
7. **Logging**: Log success/failure

#### Handler Implementation
```csharp
public class WalletChargePaymentCompletedConsumer : INotificationHandler<BillFullyPaidEvent>
{
    public async Task Handle(BillFullyPaidEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Check if this is a wallet deposit bill
            if (!IsWalletDepositBill(notification.ReferenceType))
            {
                _logger.LogDebug("Bill {BillId} is not a wallet deposit bill, skipping", notification.BillId);
                return;
            }

            // 2. Find the deposit
            if (!Guid.TryParse(notification.ReferenceId, out var depositId))
            {
                _logger.LogError("Invalid deposit ID format: {ReferenceId}", notification.ReferenceId);
                return;
            }

            var deposit = await _walletDepositRepository.GetByIdAsync(depositId, cancellationToken);
            if (deposit == null)
            {
                _logger.LogError("Deposit not found: {DepositId}", depositId);
                return;
            }

            // 3. Check deposit status
            if (deposit.Status != WalletDepositStatus.Pending)
            {
                _logger.LogWarning("Deposit {DepositId} is not pending, status: {Status}", depositId, deposit.Status);
                return;
            }

            // 4. Complete the deposit
            deposit.Complete();
            await _walletDepositRepository.UpdateAsync(deposit, cancellationToken: cancellationToken);

            // 5. Charge the wallet
            var chargeCommand = new ChargeWalletCommand
            {
                UserNationalNumber = notification.UserNationalNumber,
                AmountRials = deposit.Amount.AmountRials,
                ReferenceId = deposit.Id.ToString(),
                ExternalReference = notification.LastGateway,
                Description = $"Wallet deposit completion via bill payment - DepositId: {depositId}",
                Metadata = new Dictionary<string, string>
                {
                    ["DepositId"] = depositId.ToString(),
                    ["BillId"] = notification.BillId.ToString(),
                    ["BillNumber"] = notification.BillNumber,
                    ["PaymentCount"] = notification.PaymentCount.ToString(),
                    ["LastPaymentMethod"] = notification.LastPaymentMethod ?? string.Empty,
                    ["LastGateway"] = notification.LastGateway ?? string.Empty,
                    ["FullyPaidDate"] = notification.FullyPaidDate.ToString("O")
                }
            };

            var result = await _mediator.Send(chargeCommand, cancellationToken);

            // 6. Log results
            if (result.IsSuccess)
            {
                _logger.LogInformation("Successfully completed deposit {DepositId} and charged wallet for user {UserNationalNumber}", 
                    depositId, notification.UserNationalNumber);
            }
            else
            {
                _logger.LogError("Failed to charge wallet for user {UserNationalNumber} after completing deposit {DepositId}. Errors: {Errors}", 
                    notification.UserNationalNumber, depositId, string.Join(", ", result.Errors));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing wallet charge for BillId: {BillId}, ReferenceId: {ReferenceId}", 
                notification.BillId, notification.ReferenceId);
        }
    }

    private static bool IsWalletDepositBill(string referenceType)
    {
        return !string.IsNullOrEmpty(referenceType) && 
               string.Equals(referenceType, "WalletDeposit", StringComparison.OrdinalIgnoreCase);
    }
}
```

## 📊 Performance Considerations

### Query Optimization
- **Selective Loading**: Load only required data
- **Pagination**: Use pagination for large datasets
- **Indexing**: Proper database indexing
- **Caching**: Cache frequently accessed data

### Transaction Management
- **Unit of Work**: Proper transaction management
- **Rollback Handling**: Handle transaction failures
- **Concurrency**: Handle concurrent operations
- **Deadlock Prevention**: Avoid deadlocks

### Error Handling
- **Graceful Degradation**: Handle errors gracefully
- **User-Friendly Messages**: Provide clear error messages
- **Logging**: Comprehensive logging
- **Monitoring**: Monitor error rates

## 🔐 Security Features

### Input Validation
- **Data Type Validation**: Ensure correct data types
- **Range Validation**: Validate numeric ranges
- **Format Validation**: Validate string formats
- **Business Rule Validation**: Validate business rules

### Authorization
- **User Context**: Validate user permissions
- **Resource Access**: Check resource access rights
- **Operation Permissions**: Validate operation permissions
- **Audit Logging**: Log all operations

### Data Protection
- **Sensitive Data**: Protect sensitive information
- **Encryption**: Encrypt sensitive data
- **Access Control**: Control data access
- **Compliance**: Ensure regulatory compliance

---

This comprehensive features documentation provides detailed information about all implemented features in the Finance Application layer, including business logic, implementation details, and best practices.
