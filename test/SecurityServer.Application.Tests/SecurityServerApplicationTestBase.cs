using Xunit.Abstractions;

namespace SecurityServer;

public abstract partial class SecurityServerApplicationTestBase : SecurityServerTestBase<SecurityServerApplicationTestModule>
{

    public readonly ITestOutputHelper Output;
    protected SecurityServerApplicationTestBase(ITestOutputHelper output) : base(output)
    {
        Output = output;
    }
}