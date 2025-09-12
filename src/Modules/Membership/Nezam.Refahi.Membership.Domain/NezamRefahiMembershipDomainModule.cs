using Bonyan.Modularity.Abstractions;
using Nezam.Refahi.Shared.Domain;

namespace Nezam.Refahi.Membership.Domain;

public class NezamRefahiMembershipDomainModule : BonModule
{
  public NezamRefahiMembershipDomainModule()
  {
    DependOn<NezamRefahiSharedDomainModule>();
  }
}