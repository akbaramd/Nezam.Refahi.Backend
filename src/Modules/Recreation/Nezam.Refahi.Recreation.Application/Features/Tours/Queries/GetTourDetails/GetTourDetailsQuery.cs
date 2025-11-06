using MediatR;
using Nezam.Refahi.Recreation.Contracts.Dtos;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Recreation.Application.Features.Tours.Queries.GetTourDetails;

public record GetTourDetailsQuery(Guid TourId, Guid? ExternalUserId = null) : IRequest<ApplicationResult<TourDetailWithUserReservationDto>>;


