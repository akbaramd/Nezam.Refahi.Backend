using FluentValidation;
using MCA.SharedKernel.Application.Contracts;
using MediatR;
using Nezam.Refahi.Finance.Application.DTOs;
using Nezam.Refahi.Finance.Application.Queries.Bills;
using Nezam.Refahi.Finance.Domain.Entities;
using Nezam.Refahi.Finance.Domain.Repositories;
using Nezam.Refahi.Finance.Domain.Enums;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Finance.Application.Features.Bills.Queries.GetBillPaymentStatus;

/// <summary>
/// Handler for GetBillPaymentStatusQuery - Retrieves bill payment status and related information
/// </summary>
/// <summary>
    /// Retrieves complete bill details with relations (items, payments, refunds) using mappers.
    /// </summary>
    public sealed class GetBillDetailsQueryHandler :
        IRequestHandler<GetBillDetailsByIdQuery, ApplicationResult<BillDetailDto>>,
        IRequestHandler<GetBillDetailsByNumberQuery, ApplicationResult<BillDetailDto>>,
        IRequestHandler<GetBillDetailsByTrackingCodeQuery, ApplicationResult<BillDetailDto>>
    {
        private readonly IBillRepository _billRepository;
        private readonly IMapper<Bill, BillDetailDto> _billDetailMapper;
        private readonly IValidator<GetBillDetailsByIdQuery> _byIdValidator;
        private readonly IValidator<GetBillDetailsByNumberQuery> _byNumberValidator;
        private readonly IValidator<GetBillDetailsByTrackingCodeQuery> _byTrackValidator;

        public GetBillDetailsQueryHandler(
            IBillRepository billRepository,
            IMapper<Domain.Entities.Bill, BillDetailDto> billDetailMapper,
            IValidator<GetBillDetailsByIdQuery> byIdValidator,
            IValidator<GetBillDetailsByNumberQuery> byNumberValidator,
            IValidator<GetBillDetailsByTrackingCodeQuery> byTrackValidator)
        {
            _billRepository   = billRepository   ?? throw new ArgumentNullException(nameof(billRepository));
            _billDetailMapper = billDetailMapper ?? throw new ArgumentNullException(nameof(billDetailMapper));
            _byIdValidator    = byIdValidator    ?? throw new ArgumentNullException(nameof(byIdValidator));
            _byNumberValidator= byNumberValidator?? throw new ArgumentNullException(nameof(byNumberValidator));
            _byTrackValidator = byTrackValidator ?? throw new ArgumentNullException(nameof(byTrackValidator));
        }

        // ------------------------ by Id ------------------------
        public async Task<ApplicationResult<BillDetailDto>> Handle(GetBillDetailsByIdQuery request, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            var validation = await _byIdValidator.ValidateAsync(request, ct);
            if (!validation.IsValid)
                return ApplicationResult<BillDetailDto>.Failure(validation.Errors.Select(e => e.ErrorMessage).ToList(), "Validation failed.");

            // Load aggregate with relations once (no N+1)
            var bill = await _billRepository.GetWithAllDataAsync(request.BillId, ct);
            if (bill is null)
                return ApplicationResult<BillDetailDto>.Failure("Bill not found.");

            var dto = await _billDetailMapper.MapAsync(bill, ct);
            return ApplicationResult<BillDetailDto>.Success(dto, "Bill details retrieved.");
        }

        // --------------------- by BillNumber -------------------
        public async Task<ApplicationResult<BillDetailDto>> Handle(GetBillDetailsByNumberQuery request, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            var validation = await _byNumberValidator.ValidateAsync(request, ct);
            if (!validation.IsValid)
                return ApplicationResult<BillDetailDto>.Failure(validation.Errors.Select(e => e.ErrorMessage).ToList(), "Validation failed.");

            // First find id by number (indexed/light), then load full graph by id
            var basic = await _billRepository.GetByBillNumberAsync(request.BillNumber, ct);
            if (basic is null)
                return ApplicationResult<BillDetailDto>.Failure("Bill not found.");

            var bill = await _billRepository.GetWithAllDataAsync(basic.Id, ct);
            if (bill is null)
                return ApplicationResult<BillDetailDto>.Failure("Bill data not found.");

            var dto = await _billDetailMapper.MapAsync(bill, ct);
            return ApplicationResult<BillDetailDto>.Success(dto, "Bill details retrieved.");
        }

        // ---------------- by TrackingCode + BillType ----------
        public async Task<ApplicationResult<BillDetailDto>> Handle(GetBillDetailsByTrackingCodeQuery request, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            var validation = await _byTrackValidator.ValidateAsync(request, ct);
            if (!validation.IsValid)
                return ApplicationResult<BillDetailDto>.Failure(validation.Errors.Select(e => e.ErrorMessage).ToList(), "Validation failed.");

            // Resolve bill id by reference(TrackingCode) + BillType; then load full graph
            var basic = await _billRepository.GetByReferenceAsync(request.TrackingCode, request.BillType, ct);
            if (basic is null)
                return ApplicationResult<BillDetailDto>.Failure("Bill not found for the given tracking code.");

            var bill = await _billRepository.GetWithAllDataAsync(basic.Id, ct);
            if (bill is null)
                return ApplicationResult<BillDetailDto>.Failure("Bill data not found.");

            var dto = await _billDetailMapper.MapAsync(bill, ct);
            return ApplicationResult<BillDetailDto>.Success(dto, "Bill details retrieved.");
        }
    }