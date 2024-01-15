using System.Collections.Generic;
using AElf;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using SecurityServer.Options;
using SecurityServer.Providers;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace SecurityServer.Storage;

public class SignatureTest : SecurityServerApplicationTestBase
{
    public SignatureTest(ITestOutputHelper output) : base(output)
    {
    }

    protected override void AfterAddApplication(IServiceCollection services)
    {
        services.AddSingleton(MockKeyStoreOptions());
    }


    [Fact]
    public void AccountProviderTest()
    {
        var accountProvider = GetRequiredService<AccountProvider>();
        var txId = HashHelper.ComputeFrom("");
        var sign = accountProvider.GetSignature("4yK5iHxrRYPj7C2JwG938Ss24zq9EMgjgX182S5MJxuN9q32P", txId.ToByteArray());
        sign.ShouldNotBeNull();
        sign.ShouldBe("31a34d11a1aaf7691eea92be32f37025545b1f38d64e849fbca73599cf551c5a0a382d97e05edbc1d68575a1d01833027d8abf711458f6e3e049520c305d97b901");
    }
    
    
}