using Bonyan.Modularity.Abstractions;
using Nezam.Refahi.Shared.Application;

namespace Nezam.Refahi.Facilities.Contracts;

public class NezamRefahiFacilitiesContractsModule : BonModule
{
  public NezamRefahiFacilitiesContractsModule()
  {
    DependOn<NezamRefahiSharedApplicationModule>();
  }
}
