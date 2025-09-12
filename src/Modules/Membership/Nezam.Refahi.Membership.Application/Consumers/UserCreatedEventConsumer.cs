using MediatR;
using Nezam.Refahi.Identity.Contracts.Events;
using Nezam.Refahi.Membership.Application.Services;
using Nezam.Refahi.Membership.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Interfaces;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Membership.Application.Consumers;

public class UserCreatedEventConsumer : INotificationHandler<UserCreatedEvent>
{

  private readonly IMemberRepository _memberRepository;
  private readonly IMembershipUnitOfWork _membershipUnitOfWork;
  public UserCreatedEventConsumer(IMemberRepository memberRepository, IMembershipUnitOfWork membershipUnitOfWork)
  {
    _memberRepository = memberRepository;
    _membershipUnitOfWork = membershipUnitOfWork;
  }

  public async Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
  {
    if (string.IsNullOrWhiteSpace(notification.NationalId))
      return;

    try
    {
      var existingMember = await _memberRepository.GetByNationalCodeAsync(new NationalId(notification.NationalId));
      if (existingMember == null)
        return;

      existingMember.SetUserId(notification.UserId);
      await _memberRepository.UpdateAsync(existingMember, cancellationToken);
      await _membershipUnitOfWork.SaveChangesAsync(cancellationToken);
    }
    catch (Exception ex)
    {
      // Log error but don't throw to avoid breaking the event handling pipeline
      Console.WriteLine($"Error handling UserCreatedEvent for user {notification.UserId}: {ex.Message}");
    }
  }
}
