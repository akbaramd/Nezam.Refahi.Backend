using MCA.SharedKernel.Domain.Models;
using MediatR;
using Nezam.Refahi.Finance.Application.DTOs;
using Nezam.Refahi.Finance.Domain.Entities;
using Nezam.Refahi.Finance.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Models;
using Microsoft.Extensions.Logging;
using FluentValidation;
using MCA.SharedKernel.Application.Contracts;

namespace Nezam.Refahi.Finance.Application.Features.Payments.Queries.GetPaymentDetail;

/// <summary>
/// Handler for retrieving detailed information of a payment.
/// </summary>
public sealed class GetPaymentDetailQueryHandler
  : IRequestHandler<GetPaymentDetailQuery, ApplicationResult<PaymentDetailDto>>
{
  private readonly IPaymentRepository _paymentRepository;
  private readonly IValidator<GetPaymentDetailQuery> _validator;
  private readonly IMapper<Payment,PaymentDetailDto> _mapper;
  private readonly ILogger<GetPaymentDetailQueryHandler> _logger;

  public GetPaymentDetailQueryHandler(
    IPaymentRepository paymentRepository,
    IValidator<GetPaymentDetailQuery> validator,
    ILogger<GetPaymentDetailQueryHandler> logger, IMapper<Payment, PaymentDetailDto> mapper)
  {
    _paymentRepository = paymentRepository;
    _validator = validator;
    _logger = logger;
    _mapper = mapper;
  }

  public async Task<ApplicationResult<PaymentDetailDto>> Handle(
    GetPaymentDetailQuery request,
    CancellationToken cancellationToken)
  {
    // Validate request
    var validation = await _validator.ValidateAsync(request, cancellationToken);
    if (!validation.IsValid)
    {
      var errors = validation.Errors.Select(e => e.ErrorMessage).ToList();
      return ApplicationResult<PaymentDetailDto>.Failure(errors, "درخواست نامعتبر است.");
    }

    try
    {
      _logger.LogInformation("Retrieving payment details for PaymentId={PaymentId}, ExternalUserId={ExternalUserId}", 
        request.PaymentId, request.ExternalUserId);

      // Fetch entity with Bill navigation
      var payment = await _paymentRepository.GetByIdAsync(request.PaymentId, cancellationToken);
      if (payment == null)
        return ApplicationResult<PaymentDetailDto>.NotFound("پرداخت مورد نظر یافت نشد.");

      // Check ownership: ensure the payment's bill belongs to the requesting user
      if (payment.Bill == null)
      {
        _logger.LogWarning("Payment {PaymentId} has no associated bill", request.PaymentId);
        return ApplicationResult<PaymentDetailDto>.NotFound("صورت حساب مرتبط با این پرداخت یافت نشد.");
      }

      if (payment.Bill.ExternalUserId != request.ExternalUserId)
      {
        _logger.LogWarning("User {UserId} attempted to access payment {PaymentId} owned by {OwnerId}",
          request.ExternalUserId, request.PaymentId, payment.Bill.ExternalUserId);
        return ApplicationResult<PaymentDetailDto>.Forbidden("شما دسترسی به این پرداخت ندارید.");
      }

      // Map to DTO
      var dto = await _mapper.MapAsync(payment, cancellationToken);

      return ApplicationResult<PaymentDetailDto>.Success(dto);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error retrieving payment detail for PaymentId={PaymentId}", request.PaymentId);
      return ApplicationResult<PaymentDetailDto>.Failure(ex, "خطا در دریافت جزئیات پرداخت");
    }
  }

  /* -------------------------------------------------------------------
     Mapping Helpers
  ------------------------------------------------------------------- */
}