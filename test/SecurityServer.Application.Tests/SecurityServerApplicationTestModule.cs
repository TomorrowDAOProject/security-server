using Volo.Abp.AutoMapper;
using Volo.Abp.EventBus;
using Volo.Abp.Modularity;

namespace SecurityServer;

[DependsOn(
    typeof(AbpEventBusModule),
    typeof(SecurityServerApplicationModule),
    typeof(SecurityServerApplicationContractsModule)
)]
public class SecurityServerApplicationTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        base.ConfigureServices(context);
        Configure<AbpAutoMapperOptions>(options => { options.AddMaps<SecurityServerApplicationModule>(); });
    }
}