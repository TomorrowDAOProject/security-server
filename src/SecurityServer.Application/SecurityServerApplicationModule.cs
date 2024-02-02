using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.DependencyInjection;
using SecurityServer.Options;
using SecurityServer.Providers;
using SecurityServer.Providers.ExecuteStrategy;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace SecurityServer;

[DependsOn(
    typeof(SecurityServerDomainModule),
    typeof(SecurityServerApplicationContractsModule)
)]
public class SecurityServerApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        Configure<AbpAutoMapperOptions>(options => { options.AddMaps<SecurityServerApplicationModule>(); });
        Configure<KeyPairInfoOptions>(configuration.GetSection("KeyPairInfo"));
        Configure<KeyStoreOptions>(configuration.GetSection("KeyStore"));
        
        
        context.Services.AddSingleton<AccountProvider>();
        context.Services.AddSingleton<StorageProvider>();
        
        context.Services.AddSingleton<AlchemyPayAesSignStrategy>();
        context.Services.AddSingleton<AlchemyPayShaSignStrategy>();
        context.Services.AddSingleton<AlchemyPayHmacSignStrategy>();
        context.Services.AddSingleton<AppleAuthStrategy>();
        
        context.Services.AddScoped<JwtSecurityTokenHandler>();
        
        context.Services.AddHttpClient();
    }
    
}