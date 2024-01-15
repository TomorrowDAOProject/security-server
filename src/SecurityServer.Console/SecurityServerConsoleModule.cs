using Volo.Abp.Modularity;

namespace SecurityServer;

[DependsOn(
    typeof(SecurityServerApplicationContractsModule)
)]
public class SecurityServerConsoleModule : AbpModule
{
    
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
    }
    
}
