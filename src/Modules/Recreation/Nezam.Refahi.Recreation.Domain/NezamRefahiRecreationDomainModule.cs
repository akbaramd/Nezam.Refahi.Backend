using Bonyan.Modularity.Abstractions;
using Nezam.Refahi.Shared.Domain;

namespace Nezam.Refahi.Recreation.Domain;

public class NezamRefahiRecreationDomainModule : BonModule
{
  public NezamRefahiRecreationDomainModule()
  {
    DependOn<NezamRefahiSharedDomainModule>();
  }
}