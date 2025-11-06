using FluentValidation;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Finance.Contracts.Services;
using Nezam.Refahi.Finance.Domain.Entities;
using Nezam.Refahi.Finance.Domain.Enums;
using Nezam.Refahi.Finance.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Interfaces;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Finance.Application.Services;

/// <summary>
/// Service for bill creation and issuing operations
/// Orchestrates bill-related use cases following DDD principles
/// </summary>
public class BillService : IBillService
{
    private readonly IBillRepository _billRepository;
    private readonly IFinanceUnitOfWork _unitOfWork;
    private readonly ILogger<BillService> _logger;
    private readonly IValidator<CreateBillRequest> _createBillRequestValidator;
    private readonly ICurrentUserService _currentUserService;

    public BillService(
        IBillRepository billRepository,
        IFinanceUnitOfWork unitOfWork,
        ILogger<BillService> logger,
        IValidator<CreateBillRequest> createBillRequestValidator,
        ICurrentUserService currentUserService)
    {
        _billRepository = billRepository ?? throw new ArgumentNullException(nameof(billRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _createBillRequestValidator = createBillRequestValidator ?? throw new ArgumentNullException(nameof(createBillRequestValidator));
        _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
    }

    /// <summary>
    /// Creates a new bill in draft status
    /// </summary>
    public async Task<ApplicationResult<BillCreationResult>> CreateBillAsync(
        CreateBillRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var currentUserId = _currentUserService.UserId;
            if (currentUserId is null || currentUserId == Guid.Empty)
            {
                return ApplicationResult<BillCreationResult>.Failure("کاربر احراز هویت نشده است یا شناسه کاربر معتبر نیست");
            }

            _logger.LogInformation("Creating bill for ReferenceId: {ReferenceId}, Type: {BillType}, UserId: {UserId}",
                request.ReferenceId, request.BillType, currentUserId.Value);

            // Validate input
            var validationResult = await _createBillRequestValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                _logger.LogWarning("Bill creation validation failed: {Errors}",
                    string.Join(", ", errors));
                return ApplicationResult<BillCreationResult>.Failure(
                    errors,
                    "اطلاعات ورودی برای ایجاد فاکتور نامعتبر است");
            }

            // Begin transaction
            await _unitOfWork.BeginAsync(cancellationToken);

            try
            {
                // Idempotency/duplicate policy by ReferenceTrackingCode
                if (!string.IsNullOrWhiteSpace(request.ReferenceTrackingCode))
                {
                    var existingBill = await _billRepository.GetByReferenceTrackingCodeAsync(
                        request.ReferenceTrackingCode,
                        cancellationToken);
                    if (existingBill != null)
                    {
                        // If overdue, cancel and return the same bill (as per policy)
                        if (existingBill.Status == BillStatus.Overdue)
                        {
                            _logger.LogInformation("Existing overdue bill found. Cancelling it and returning same bill. TrackingCode={TrackingCode}, BillId={BillId}",
                                request.ReferenceTrackingCode, existingBill.Id);
                            existingBill.Cancel("لغو خودکار به دلیل درخواست ایجاد فاکتور جدید");
                            await _billRepository.UpdateAsync(existingBill, cancellationToken: cancellationToken);
                            await _unitOfWork.SaveAsync(cancellationToken);
                        }

                        // For any existing bill (Draft/Issued/PartiallyPaid/Cancelled/Overdue/etc.) return the existing bill instead of creating a new one
                        var reuseResult = new BillCreationResult
                        {
                            BillId = existingBill.Id,
                            BillNumber = existingBill.BillNumber,
                            Status = existingBill.Status.ToString(),
                            IssueDate = existingBill.IssueDate,
                            TotalAmountRials = existingBill.TotalAmount.AmountRials,
                            Currency = existingBill.TotalAmount.Currency
                        };

                        await _unitOfWork.CommitAsync(cancellationToken);
                        return ApplicationResult<BillCreationResult>.Success(
                            reuseResult,
                            "فاکتور موجود بازگردانده شد و فاکتور جدید ایجاد نشد");
                    }
                }

                // Validate items before creating bill
                if (request.Items == null || !request.Items.Any())
                {
                    _logger.LogWarning("Cannot create bill without items for ReferenceId: {ReferenceId}",
                        request.ReferenceId);
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return ApplicationResult<BillCreationResult>.Failure(
                        "فاکتور باید حداقل یک قلم داشته باشد");
                }

                // Create bill items (following the same pattern as CreateBillCommandHandler)
                // EF Core will handle the BillId relationship through navigation properties
                var billItems = request.Items.Select(item => new BillItem(
                    billId: Guid.NewGuid(), // Temporary ID - EF Core will set correct ID through navigation property
                    title: item.Title,
                    description: item.Description,
                    unitPrice: Money.FromRials(item.UnitPriceRials),
                    quantity: item.Quantity,
                    discountPercentage: item.DiscountPercentage
                )).ToList();

                // Create bill entity (Bill constructor generates its own ID)
                // EF Core will sync BillItem.BillId through the navigation property relationship
                var bill = new Bill(
                    title: request.Title,
                    referenceTrackingCode: request.ReferenceTrackingCode,
                    referenceId: request.ReferenceId,
                    billType: request.BillType,
                    externalUserId: currentUserId.Value,
                    userFullName: request.UserFullName,
                    description: request.Description,
                    dueDate: request.DueDate,
                    metadata: request.Metadata,
                    items: billItems
                );

                // Save bill
                await _billRepository.AddAsync(bill, cancellationToken: cancellationToken);
                await _unitOfWork.SaveAsync(cancellationToken);

                // Commit transaction
                await _unitOfWork.CommitAsync(cancellationToken);

                _logger.LogInformation("Bill created successfully. BillId: {BillId}, BillNumber: {BillNumber}, TotalAmount: {TotalAmount}",
                    bill.Id, bill.BillNumber, bill.TotalAmount.AmountRials);

                // Prepare response
                var result = new BillCreationResult
                {
                    BillId = bill.Id,
                    BillNumber = bill.BillNumber,
                    Status = bill.Status.ToString(),
                    IssueDate = bill.IssueDate,
                    TotalAmountRials = bill.TotalAmount.AmountRials,
                    Currency = bill.TotalAmount.Currency
                };

                return ApplicationResult<BillCreationResult>.Success(result, "فاکتور با موفقیت ایجاد شد");
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Invalid argument while creating bill for ReferenceId: {ReferenceId}",
                    request.ReferenceId);
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<BillCreationResult>.Failure(
                    $"خطا در ایجاد فاکتور: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Invalid operation while creating bill for ReferenceId: {ReferenceId}",
                    request.ReferenceId);
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<BillCreationResult>.Failure(
                    $"عملیات نامعتبر: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while creating bill for ReferenceId: {ReferenceId}",
                    request.ReferenceId);
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<BillCreationResult>.Failure(
                    "خطای داخلی در ایجاد فاکتور");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Critical error in CreateBillAsync for ReferenceId: {ReferenceId}",
                request.ReferenceId);
            return ApplicationResult<BillCreationResult>.Failure(
                "خطای غیرمنتظره در ایجاد فاکتور");
        }
    }

    /// <summary>
    /// Issues a bill (finalizes it and makes it ready for payment)
    /// </summary>
    public async Task<ApplicationResult<BillIssueResult>> IssueBillAsync(
        Guid billId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Issuing bill. BillId: {BillId}", billId);

            if (billId == Guid.Empty)
            {
                return ApplicationResult<BillIssueResult>.Failure(
                    "شناسه فاکتور نمی‌تواند خالی باشد");
            }

            // Begin transaction
            await _unitOfWork.BeginAsync(cancellationToken);

            try
            {
                // Get bill with items
                var bill = await _billRepository.GetWithItemsAsync(billId, cancellationToken);
                if (bill == null)
                {
                    _logger.LogWarning("Bill not found. BillId: {BillId}", billId);
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return ApplicationResult<BillIssueResult>.Failure(
                        "فاکتور مورد نظر یافت نشد");
                }

                // Validate bill can be issued
                if (bill.Status != BillStatus.Draft)
                {
                    _logger.LogWarning("Cannot issue bill in status {Status}. BillId: {BillId}",
                        bill.Status, billId);
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return ApplicationResult<BillIssueResult>.Failure(
                        $"فاکتور در وضعیت {bill.Status} قابل صدور نیست. فقط فاکتورهای پیش‌نویس قابل صدور هستند");
                }

                if (!bill.Items.Any())
                {
                    _logger.LogWarning("Cannot issue bill without items. BillId: {BillId}", billId);
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return ApplicationResult<BillIssueResult>.Failure(
                        "فاکتور بدون قلم قابل صدور نیست");
                }

                // Issue the bill (domain method handles status change and validation)
                bill.Issue();

                // Save changes
                await _billRepository.UpdateAsync(bill, cancellationToken: cancellationToken);
                await _unitOfWork.SaveAsync(cancellationToken);

                // Commit transaction
                await _unitOfWork.CommitAsync(cancellationToken);

                _logger.LogInformation("Bill issued successfully. BillId: {BillId}, BillNumber: {BillNumber}, Status: {Status}",
                    bill.Id, bill.BillNumber, bill.Status);

                // Prepare response
                var result = new BillIssueResult
                {
                    BillId = bill.Id,
                    BillNumber = bill.BillNumber,
                    Status = bill.Status.ToString(),
                    IssueDate = bill.IssueDate,
                    TotalAmountRials = bill.TotalAmount.AmountRials,
                    Currency = bill.TotalAmount.Currency,
                    RemainingAmountRials = bill.RemainingAmount.AmountRials
                };

                return ApplicationResult<BillIssueResult>.Success(result, "فاکتور با موفقیت صادر شد");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Invalid operation while issuing bill. BillId: {BillId}", billId);
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<BillIssueResult>.Failure(
                    $"عملیات نامعتبر: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while issuing bill. BillId: {BillId}", billId);
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<BillIssueResult>.Failure(
                    "خطای داخلی در صدور فاکتور");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Critical error in IssueBillAsync. BillId: {BillId}", billId);
            return ApplicationResult<BillIssueResult>.Failure(
                "خطای غیرمنتظره در صدور فاکتور");
        }
    }

    /// <summary>
    /// Creates and immediately issues a bill in one operation
    /// </summary>
    public async Task<ApplicationResult<BillIssueResult>> CreateAndIssueBillAsync(
        CreateBillRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating and issuing bill in one operation. ReferenceId: {ReferenceId}",
                request.ReferenceId);

            // Idempotency/duplicate policy by ReferenceTrackingCode
            if (!string.IsNullOrWhiteSpace(request.ReferenceTrackingCode))
            {
                var existingBill = await _billRepository.GetByReferenceTrackingCodeAsync(
                    request.ReferenceTrackingCode,
                    cancellationToken);
                if (existingBill != null)
                {
                    // If overdue, cancel and return existing
                    if (existingBill.Status == BillStatus.Overdue)
                    {
                        _logger.LogInformation("Existing overdue bill found (CreateAndIssue). Cancelling. TrackingCode={TrackingCode}, BillId={BillId}",
                            request.ReferenceTrackingCode, existingBill.Id);
                        await _unitOfWork.BeginAsync(cancellationToken);
                        try
                        {
                            existingBill.Cancel("لغو خودکار به دلیل درخواست صدور مجدد");
                            await _billRepository.UpdateAsync(existingBill, cancellationToken: cancellationToken);
                            await _unitOfWork.SaveAsync(cancellationToken);
                            await _unitOfWork.CommitAsync(cancellationToken);
                        }
                        catch
                        {
                            await _unitOfWork.RollbackAsync(cancellationToken);
                            // continue
                        }
                    }

                    // If already issued or partially paid, return existing as result (do not issue new)
                    if (existingBill.Status == BillStatus.Issued || existingBill.Status == BillStatus.PartiallyPaid)
                    {
                        var existingResult = new BillIssueResult
                        {
                            BillId = existingBill.Id,
                            BillNumber = existingBill.BillNumber,
                            Status = existingBill.Status.ToString(),
                            IssueDate = existingBill.IssueDate,
                            TotalAmountRials = existingBill.TotalAmount.AmountRials,
                            Currency = existingBill.TotalAmount.Currency,
                            RemainingAmountRials = existingBill.RemainingAmount.AmountRials
                        };
                        return ApplicationResult<BillIssueResult>.Success(existingResult, "فاکتور موجود بازگردانده شد و دوباره صادر نشد");
                    }

                    // If cancelled, try reopening to draft and issuing
                    if (existingBill.Status == BillStatus.Cancelled)
                    {
                        _logger.LogInformation("Reopening cancelled bill to draft for re-issuing. BillId={BillId}", existingBill.Id);
                        await _unitOfWork.BeginAsync(cancellationToken);
                        try
                        {
                            // Refresh due date for reuse: prefer request.DueDate; else fallback to +7 days handled inside domain
                            existingBill.ReopenToDraft("بازگشایی جهت صدور مجدد", request.DueDate);
                            await _billRepository.UpdateAsync(existingBill, cancellationToken: cancellationToken);
                            await _unitOfWork.SaveAsync(cancellationToken);
                            await _unitOfWork.CommitAsync(cancellationToken);
                        }
                        catch (Exception)
                        {
                            await _unitOfWork.RollbackAsync(cancellationToken);
                            // continue
                        }

                        return await IssueBillAsync(existingBill.Id, cancellationToken);
                    }

                    // If draft exists, try issuing the same bill
                    if (existingBill.Status == BillStatus.Draft)
                    {
                        // If draft exists, ensure due date is still valid; refresh if needed
                        if (!existingBill.DueDate.HasValue || existingBill.DueDate.Value <= DateTime.UtcNow)
                        {
                            await _unitOfWork.BeginAsync(cancellationToken);
                            try
                            {
                                existingBill.ReopenToDraft("به‌روزرسانی تاریخ سررسید پیش از صدور", request.DueDate);
                                await _billRepository.UpdateAsync(existingBill, cancellationToken: cancellationToken);
                                await _unitOfWork.SaveAsync(cancellationToken);
                                await _unitOfWork.CommitAsync(cancellationToken);
                            }
                            catch
                            {
                                await _unitOfWork.RollbackAsync(cancellationToken);
                            }
                        }
                        return await IssueBillAsync(existingBill.Id, cancellationToken);
                    }

                    // For other terminal statuses (Cancelled, Voided, FullyPaid, etc.) fallthrough to create new
                }
            }

            // Create bill first (no duplicate found or allowed to create new)
            var createResult = await CreateBillAsync(request, cancellationToken);
            if (!createResult.IsSuccess)
            {
                return ApplicationResult<BillIssueResult>.Failure(
                    createResult.Errors,      createResult.Message ?? "خطا در ایجاد فاکتور");
            }

            // Issue the created bill
            var issueResult = await IssueBillAsync(createResult.Data!.BillId, cancellationToken);
            if (!issueResult.IsSuccess)
            {
                _logger.LogWarning("Bill created but failed to issue. BillId: {BillId}, Error: {Error}",
                    createResult.Data.BillId, issueResult.Message);
                return ApplicationResult<BillIssueResult>.Failure(
                    $"فاکتور ایجاد شد اما در صدور آن خطا رخ داد: {issueResult.Message}");
            }

            _logger.LogInformation("Bill created and issued successfully. BillId: {BillId}",
                issueResult.Data!.BillId);

            return ApplicationResult<BillIssueResult>.Success(
                issueResult.Data,
                "فاکتور با موفقیت ایجاد و صادر شد");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Critical error in CreateAndIssueBillAsync for ReferenceId: {ReferenceId}",
                request.ReferenceId);
            return ApplicationResult<BillIssueResult>.Failure(
                "خطای غیرمنتظره در ایجاد و صدور فاکتور");
        }
    }

    /// <summary>
    /// Cancels a bill and expires associated payments
    /// </summary>
    public async Task<ApplicationResult<BillCancellationResult>> CancelBillAsync(
        Guid billId,
        string? reason = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Cancelling bill. BillId: {BillId}, Reason: {Reason}", billId, reason);

            if (billId == Guid.Empty)
            {
                return ApplicationResult<BillCancellationResult>.Failure(
                    "شناسه فاکتور نمی‌تواند خالی باشد");
            }

            // Begin transaction
            await _unitOfWork.BeginAsync(cancellationToken);

            try
            {
                // Get bill
                var bill = await _billRepository.GetByIdAsync(billId, cancellationToken);
                if (bill == null)
                {
                    _logger.LogWarning("Bill not found. BillId: {BillId}", billId);
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return ApplicationResult<BillCancellationResult>.Failure(
                        "فاکتور مورد نظر یافت نشد");
                }

                // Cancel the bill (domain behavior)
                bill.Cancel(reason);

                // Save changes
                await _billRepository.UpdateAsync(bill, cancellationToken: cancellationToken);
                await _unitOfWork.SaveAsync(cancellationToken);

                // Commit transaction
                await _unitOfWork.CommitAsync(cancellationToken);

                _logger.LogInformation("Bill cancelled successfully. BillId: {BillId}", billId);

                var result = new BillCancellationResult
                {
                    BillId = bill.Id,
                    BillNumber = bill.BillNumber,
                    Status = bill.Status.ToString(),
                    CancellationReason = reason,
                    CancelledAt = DateTime.UtcNow
                };

                return ApplicationResult<BillCancellationResult>.Success(result, "فاکتور با موفقیت لغو شد");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Invalid operation while cancelling bill. BillId: {BillId}", billId);
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<BillCancellationResult>.Failure(
                    $"عملیات نامعتبر: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while cancelling bill. BillId: {BillId}", billId);
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<BillCancellationResult>.Failure(
                    "خطای داخلی در لغو فاکتور");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Critical error in CancelBillAsync. BillId: {BillId}", billId);
            return ApplicationResult<BillCancellationResult>.Failure(
                "خطای غیرمنتظره در لغو فاکتور");
        }
    }
}

