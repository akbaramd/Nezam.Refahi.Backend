using System.Linq;
using FluentValidation;
using MediatR;
using MCA.SharedKernel.Application.Contracts;
using MCA.SharedKernel.Domain.Models;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Finance.Application.DTOs;
using Nezam.Refahi.Finance.Application.Features.Payments.Queries.GetPaymentsPaginated;
using Nezam.Refahi.Finance.Application.Spesifications;
using Nezam.Refahi.Finance.Domain.Entities;
using Nezam.Refahi.Finance.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Interfaces;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Finance.Application.Features.Payments.Queries.GetPaymentsPaginated;

public sealed class GetPaymentsPaginatedQueryHandler
    : IRequestHandler<GetPaymentsPaginatedQuery, ApplicationResult<PaginatedResult<PaymentDto>>>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IValidator<GetPaymentsPaginatedQuery> _validator;
    private readonly IMapper<Payment, PaymentDto> _paymentMapper;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<GetPaymentsPaginatedQueryHandler> _logger;

    public GetPaymentsPaginatedQueryHandler(
        IPaymentRepository paymentRepository,
        IValidator<GetPaymentsPaginatedQuery> validator,
        IMapper<Payment, PaymentDto> paymentMapper,
        ICurrentUserService currentUser,
        ILogger<GetPaymentsPaginatedQueryHandler> logger)
    {
        _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _paymentMapper = paymentMapper ?? throw new ArgumentNullException(nameof(paymentMapper));
        _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ApplicationResult<PaginatedResult<PaymentDto>>> Handle(
        GetPaymentsPaginatedQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Validate request
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
            {
                var errors = validation.Errors.Select(e => e.ErrorMessage).ToList();
                return ApplicationResult<PaginatedResult<PaymentDto>>.Failure(
                    errors, "درخواست نامعتبر است");
            }

            // Determine the user ID to use
            var userId = request.ExternalUserId ??
                        (_currentUser.IsAuthenticated ? _currentUser.UserId : null);

            if (!userId.HasValue)
            {
                _logger.LogWarning("GetPaymentsPaginated: No user ID available");
                return ApplicationResult<PaginatedResult<PaymentDto>>.Failure(
                    "شناسه کاربر الزامی است");
            }

            _logger.LogInformation(
                "GetPaymentsPaginated: userId={UserId} page={Page} size={Size} status={Status} search='{Search}'",
                userId.Value, request.PageNumber, request.PageSize, request.Status, request.Search);

            // Specification-based pagination via repository
            var spec = new GetPaymentsPaginatedForUserSpec(
                externalUserId: userId.Value,
                pageNumber: request.PageNumber,
                pageSize: request.PageSize,
                status: request.Status,
                search: request.Search,
                fromDate: request.FromDate,
                toDate: request.ToDate);

            var pageData = await _paymentRepository.GetPaginatedAsync(spec, cancellationToken);
            var payments = pageData.Items.ToList();

            // Map to DTOs
            var items = (await Task.WhenAll(
                payments.Select(p => _paymentMapper.MapAsync(p, cancellationToken))))
                .ToList();

            var page = new PaginatedResult<PaymentDto>
            {
                Items = items,
                TotalCount = pageData.TotalCount,
                PageNumber = pageData.PageNumber,
                PageSize = pageData.PageSize
            };

            return ApplicationResult<PaginatedResult<PaymentDto>>.Success(page);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("GetPaymentsPaginated cancelled");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetPaymentsPaginated failed");
            return ApplicationResult<PaginatedResult<PaymentDto>>.Failure(
                ex, "خطا در دریافت لیست پرداخت‌ها");
        }
    }
}

