using Bonyan.Modularity.Abstractions;
using Nezam.Refahi.Shared.Domain;

namespace Nezam.Refahi.Settings.Domain;

public class NezamRefahiSettingsDomainModule : BonModule
{
  public NezamRefahiSettingsDomainModule()
  {
    DependOn<NezamRefahiSharedDomainModule>();
  }
}
