using Volo.Abp.Modularity;

namespace SecurityServer;

[DependsOn(
    typeof(SecurityServerDomainSharedModule)
)]
public class SecurityServerApplicationContractsModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        SecurityServerDtoExtensions.Configure();
    }
}
