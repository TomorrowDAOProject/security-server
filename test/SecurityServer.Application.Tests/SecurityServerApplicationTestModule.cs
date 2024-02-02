using Microsoft.Extensions.DependencyInjection;
using SecurityServer.Controllers;
using Volo.Abp.AutoMapper;
using Volo.Abp.EventBus;
using Volo.Abp.Modularity;

namespace SecurityServer;

[DependsOn(
    typeof(AbpEventBusModule),
    typeof(SecurityServerHttpApiModule),
    typeof(SecurityServerApplicationModule)
)]
public class SecurityServerApplicationTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        base.ConfigureServices(context);
        context.Services.AddTransient<ThirdPartController>();
        Configure<AbpAutoMapperOptions>(options => { options.AddMaps<SecurityServerHttpApiModule>(); });
        Configure<AbpAutoMapperOptions>(options => { options.AddMaps<SecurityServerApplicationModule>(); });
    }
}