using Microsoft.Extensions.DependencyInjection;
using SecurityServer.Options;
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
        
        var configuration = context.Services.GetConfiguration();
        Configure<AuthorityOptions>(configuration.GetSection("Authority"));

    }
}

