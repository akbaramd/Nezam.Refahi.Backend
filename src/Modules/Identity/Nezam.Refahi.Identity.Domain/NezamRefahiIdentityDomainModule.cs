using Bonyan.Modularity.Abstractions;
using Nezam.Refahi.Shared.Domain;

namespace Nezam.Refahi.Identity.Domain;

public class NezamRefahiIdentityDomainModule : BonModule
{
  public NezamRefahiIdentityDomainModule()
  {
    DependOn<NezamRefahiSharedDomainModule>();
  }
}
