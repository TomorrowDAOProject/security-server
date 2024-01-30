using Volo.Abp.Modularity;

namespace SecurityServer;

[DependsOn(typeof(SecurityServerDomainSharedModule))]
public class SecurityServerDomainModule : AbpModule
{
    
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
    }
    
}
