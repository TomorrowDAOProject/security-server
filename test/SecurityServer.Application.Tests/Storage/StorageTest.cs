using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using SecurityServer.Dtos;
using SecurityServer.Options;
using SecurityServer.Providers;
using SecurityServer.Providers.ExecuteStrategy;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace SecurityServer.Storage;

public class StorageTest : SecurityServerApplicationTestBase
{
    public StorageTest(ITestOutputHelper output) : base(output)
    {
    }

    protected override void AfterAddApplication(IServiceCollection services)
    {
        services.AddSingleton(MockStorageOptions());
    }

    [Fact]
    public void ModTest()
    {
        var storageProvider = GetRequiredService<StorageProvider>();
        var alchemyPayAesStrategy = GetRequiredService<AlchemyPayAesSignStrategy>();
        
        var achAesSign = Assert.ThrowsAny<Exception>(() => storageProvider.ExecuteThirdPartSecret(
            new CommonThirdPartExecuteInput
            {
                Key = "AesCbcData",
                BizData = "222"
            }, alchemyPayAesStrategy));
        achAesSign.Message.ShouldBe("Permission denied");

        var testKey = Assert.ThrowsAny<Exception>(() => storageProvider.GetThirdPartSecret("TestKey"));
        testKey.Message.ShouldBe("Permission denied");
    }

    [Fact]
    public void GetStorageTest()
    {
        var storageProvider = GetRequiredService<StorageProvider>();

        var gcmSecret = storageProvider.GetThirdPartSecret("AesGcmData");
        Output.WriteLine(gcmSecret);
        gcmSecret.ShouldBe("1111111111111111");

        var cbcSecret = storageProvider.GetThirdPartSecret("AesCbcData");
        Output.WriteLine(cbcSecret);
        cbcSecret.ShouldBe("1111111111111111");

        var appleSecret = storageProvider.GetThirdPartSecret("appleMockKey");
        Output.WriteLine(cbcSecret);
        appleSecret.ShouldBe(
            "MIGHAgEAMBMGByqGSM49AgEGCCqGSM49AwEHBG0wawIBAQQgZlhOmKciniLq5vn6\\nu8a8O2GHEsFx8PJbYP5cDqljLSihRANCAASj0gesLmRK/BTFFLvuj16dnbq6VthI\\nlE6xIRKXNq+NGs2S0K9/P4jQDdPiflvto6OwFt/LnYnulBQsBiDuHd8u");
    }


    [Fact]
    public void SecretExecuteTest()
    {
        var alchemyPayAesStrategy = GetRequiredService<AlchemyPayAesSignStrategy>();
        var alchemyPayHmacStrategy = GetRequiredService<AlchemyPayHmacSignStrategy>();
        var alchemyPayShaStrategy = GetRequiredService<AlchemyPayShaSignStrategy>();
        var appleSignStrategy = GetRequiredService<AppleAuthStrategy>();
        var storageProvider = GetRequiredService<StorageProvider>();
        var achAesSign = storageProvider.ExecuteThirdPartSecret(new CommonThirdPartExecuteInput
        {
            Key = "AesGcmData",
            BizData = "222"
        }, alchemyPayAesStrategy);
        Output.WriteLine(achAesSign.Value);

        var achHmacSign = storageProvider.ExecuteThirdPartSecret(new CommonThirdPartExecuteInput
        {
            Key = "AesGcmData",
            BizData = "222"
        }, alchemyPayHmacStrategy);
        Output.WriteLine(achHmacSign.Value);

        var achShaSign = storageProvider.ExecuteThirdPartSecret(new CommonThirdPartExecuteInput
        {
            Key = "AesGcmData",
            BizData = "222"
        }, alchemyPayShaStrategy);
        Output.WriteLine(achShaSign.Value);

        var appleSign = storageProvider.ExecuteThirdPartSecret(new AppleAuthExecuteInput
        {
            Key = "appleMockKey",
            KeyId = "111",
            TeamId = "111",
            ClientId = "222"
        }, appleSignStrategy);
        Output.WriteLine(appleSign.Value);
    }


    [Fact]
    public void GenerateECDsaP256KeyPairTest()
    {
        using ECDsa ecdsa = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        byte[] privateKey = ecdsa.ExportPkcs8PrivateKey();
        Output.WriteLine(Convert.ToBase64String(privateKey));
    }
}