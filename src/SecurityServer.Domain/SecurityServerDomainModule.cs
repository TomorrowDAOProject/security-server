using Volo.Abp.AuditLogging;
using Volo.Abp.Modularity;

namespace SecurityServer;

[DependsOn(typeof(SecurityServerDomainSharedModule),
        typeof(AbpAuditLoggingDomainModule)
)]
public class SecurityServerDomainModule : AbpModule
{
    
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
    }
    
}
