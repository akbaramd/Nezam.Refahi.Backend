using Bonyan.Modularity.Abstractions;
using Nezam.Refahi.Finance.Domain;
using Nezam.Refahi.Shared.Application;
using Nezam.Refahi.Shared.Domain;

namespace Nezam.Refahi.Finance.Contracts;

public class NezamRefahiFinanceContractsModule : BonModule
{
  public NezamRefahiFinanceContractsModule()
  {
    DependOn<NezamRefahiFinanceDomainModule>();
    DependOn<NezamRefahiSharedApplicationModule>();
  }
}