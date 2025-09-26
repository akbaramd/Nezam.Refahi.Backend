using Bonyan.Modularity.Abstractions;
using Nezam.Refahi.Shared.Application;

namespace Nezam.Refahi.Finance.Contracts;

public class NezamRefahiFinanceContractsModule : BonModule
{
  public NezamRefahiFinanceContractsModule()
  {
    DependOn<NezamRefahiSharedApplicationModule>();
  }
}