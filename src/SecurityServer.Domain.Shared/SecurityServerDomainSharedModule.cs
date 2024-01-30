using Volo.Abp.Modularity;

namespace SecurityServer;

[DependsOn()]
public class SecurityServerDomainSharedModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
    }
}
