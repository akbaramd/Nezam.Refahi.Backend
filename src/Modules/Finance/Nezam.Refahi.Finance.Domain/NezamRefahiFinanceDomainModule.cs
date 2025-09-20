

using Bonyan.Modularity.Abstractions;
using Nezam.Refahi.Shared.Domain;

namespace Nezam.Refahi.Finance.Domain;


public class NezamRefahiFinanceDomainModule : BonModule
{
  public NezamRefahiFinanceDomainModule()
  {
    DependOn<NezamRefahiSharedDomainModule>();
  }
}