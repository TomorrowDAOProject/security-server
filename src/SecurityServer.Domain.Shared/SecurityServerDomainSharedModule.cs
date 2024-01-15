using Volo.Abp.AuditLogging;
using Volo.Abp.Modularity;

namespace SecurityServer;

[DependsOn(
    typeof(AbpAuditLoggingDomainSharedModule)
    )]
public class SecurityServerDomainSharedModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
    }
}
