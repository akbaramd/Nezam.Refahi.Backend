using Bonyan.Modularity.Abstractions;
using Nezam.Refahi.Recreation.Domain;

namespace Nezam.Refahi.Recreation.Contracts;

public class NezamRefahiRecreationContractsModule : BonModule
{
  public NezamRefahiRecreationContractsModule()
  {
    DependOn<NezamRefahiRecreationDomainModule>();
  }
}