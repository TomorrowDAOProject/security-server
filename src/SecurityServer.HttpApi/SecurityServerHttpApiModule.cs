using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace SecurityServer;

[DependsOn(
    typeof(SecurityServerApplicationContractsModule)
    )]
public class SecurityServerHttpApiModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(SecurityServerHttpApiModule).Assembly);
        });
    }
}

