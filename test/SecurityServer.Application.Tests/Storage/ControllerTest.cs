using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using SecurityServer.Common;
using SecurityServer.Controllers;
using SecurityServer.Dtos;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace SecurityServer.Storage;

public class ControllerTest : SecurityServerApplicationTestBase
{
    
    
    
    public ControllerTest(ITestOutputHelper output) : base(output)
    {
        
    }
    
    
    protected override void AfterAddApplication(IServiceCollection services)
    {
        services.AddSingleton(MockStorageOptions());
        services.AddSingleton(MockAuthOptions());
    }
    


    [Fact]
    public async Task GetStorageTest()
    {
        const string key = "AesGcmData";
        var storageController = GetRequiredService<ThirdPartController>();
        storageController.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
        storageController.HttpContext.Request.Headers["appid"] = "caserver";
        storageController.HttpContext.Request.Headers["signature"] = AuthorityHelper.CalculateSignature(key, "12345678");

        var res = await storageController.GetSecret(key);
        res.ShouldNotBeNull();
        res.Success.ShouldBeTrue();
        
    }
    

    [Fact]
    public async Task ExecuteTest()
    {
        const string key = "AesGcmData";
        const string data = "123";
        var storageController = GetRequiredService<ThirdPartController>();
        storageController.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
        storageController.HttpContext.Request.Headers["appid"] = "caserver";
        storageController.HttpContext.Request.Headers["signature"] = AuthorityHelper.CalculateSignature(string.Join("", key, data), "12345678");

        var resAlchemyAes = await storageController.AlchemyAesSignAsync(new CommonThirdPartExecuteInput
        {
            Key = key,
            BizData = data,
        });
        resAlchemyAes.ShouldNotBeNull();
        resAlchemyAes.Success.ShouldBeTrue();
        
        var resAlchemySha = await storageController.AlchemyShaSignAsync(new CommonThirdPartExecuteInput
        {
            Key = key,
            BizData = data,
        });
        resAlchemySha.ShouldNotBeNull();
        resAlchemySha.Success.ShouldBeTrue();
        
        var resAlchemyHmac = await storageController.AlchemyHmacSignAsync(new CommonThirdPartExecuteInput
        {
            Key = key,
            BizData = data,
        });
        resAlchemyHmac.ShouldNotBeNull();
        resAlchemyHmac.Success.ShouldBeTrue();
        
    }

    [Fact]
    public async Task AppleAuthSignTest()
    {
        
        const string key = "appleMockKey";
        var appleInput = new AppleAuthExecuteInput
        {
            Key = key,
            KeyId = "KeyId",
            TeamId = "TeamId",
            ClientId = "ClientId"
        };
        var storageController = GetRequiredService<ThirdPartController>();
        storageController.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
        storageController.HttpContext.Request.Headers["appid"] = "caserver";
        storageController.HttpContext.Request.Headers["signature"] = AuthorityHelper.CalculateSignature(
            string.Join("", key, appleInput.KeyId, appleInput.TeamId, appleInput.ClientId), 
            "12345678");

        var appleSignResult = await storageController.AppleAuthSignAsync(appleInput);
        appleSignResult.ShouldNotBeNull();
        appleSignResult.Success.ShouldBeTrue();
    }
    
    
    
}