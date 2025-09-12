using MCA.SharedKernel.Application.Contracts;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Identity.Application.Features.Users.Commands.DeleteUser;

public class DeleteUserCommand : ICommand<ApplicationResult>
{
    public Guid Id { get; set; }
    public bool SoftDelete { get; set; } = true;
    public string? DeleteReason { get; set; }
}
