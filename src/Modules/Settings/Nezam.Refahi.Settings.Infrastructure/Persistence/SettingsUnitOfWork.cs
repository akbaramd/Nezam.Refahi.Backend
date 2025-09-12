using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Settings.Application.Services;
using Nezam.Refahi.Settings.Infrastructure.Persistence;
using Nezam.Refahi.Shared.Infrastructure.Persistence;

namespace Nezam.Refahi.Settings.Infrastructure.Persistence;

public class SettingsUnitOfWork :BaseUnitOfWork<SettingsDbContext>, ISettingsUnitOfWork
{
  public SettingsUnitOfWork(SettingsDbContext context, IMediator mediator, ILogger logger) : base(context, mediator, logger)
  {
  }
}
