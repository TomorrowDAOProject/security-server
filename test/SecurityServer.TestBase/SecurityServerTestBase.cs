using Serilog;
using Volo.Abp;
using Volo.Abp.Modularity;
using Volo.Abp.Testing;
using Xunit.Abstractions;

namespace SecurityServer;

/* All test classes are derived from this class, directly or indirectly.
 */
public abstract class SecurityServerTestBase<TStartupModule> : AbpIntegratedTest<TStartupModule> where TStartupModule : IAbpModule

{
    protected readonly ITestOutputHelper Output;

    protected SecurityServerTestBase(ITestOutputHelper output)
    {
        Output = output;
        Log.Logger = new LoggerConfiguration().CreateLogger();
    }

    protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
    {
        options.UseAutofac();
    }
    
}