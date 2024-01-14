using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SecurityServer.Common;
using SecurityServer.Dtos;
using SecurityServer.Options;
using SecurityServer.Providers;
using SecurityServer.Providers.ExecuteStrategy;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace SecurityServer.Controllers;

[RemoteService]
[Area("app")]
[ControllerName("ThirdPart")]
[Route("api/app/thirdPart")]
public class ThirdPartController : AbpController
{
    private readonly StorageProvider _storageProvider;
    private readonly IOptionsMonitor<AuthorityOptions> _authOptions;
    private readonly AlchemyPayAesSignStrategy _alchemyPayAesSignStrategy;
    private readonly AlchemyPayShaSignStrategy _alchemyPayShaSignStrategy;
    private readonly AlchemyPayShaSignStrategy _alchemyPayHmacSignStrategy;
    private readonly AppleAuthStrategy _appleAuthStrategy;

    public ThirdPartController(StorageProvider storageProvider, IOptionsMonitor<AuthorityOptions> authOptions,
        AlchemyPayAesSignStrategy alchemyPayAesSignStrategy, AlchemyPayShaSignStrategy alchemyPayShaSignStrategy,
        AlchemyPayShaSignStrategy alchemyPayHmacSignStrategy, AppleAuthStrategy appleAuthStrategy)
    {
        _storageProvider = storageProvider;
        _authOptions = authOptions;
        _alchemyPayAesSignStrategy = alchemyPayAesSignStrategy;
        _alchemyPayShaSignStrategy = alchemyPayShaSignStrategy;
        _alchemyPayHmacSignStrategy = alchemyPayHmacSignStrategy;
        _appleAuthStrategy = appleAuthStrategy;
    }

    [HttpGet]
    public Task<CommonResponse<string>> GetSecret(string key)
    {
        var (_, appsecret) = AuthorityHelper.AssertDappHeader(_authOptions.CurrentValue, HttpContext, key);
        var secret = _storageProvider.GetThirdPartSecret(key);
        secret = EncryptHelper.AesCbcEncrypt(secret, appsecret);
        return Task.FromResult(new CommonResponse<string>(secret));
    }


    [HttpPost("thirdPart/alchemyAes")]
    public Task<CommonResponse<CommonThirdPartExecuteOutput>> AlchemyAesSignAsync(
        CommonThirdPartExecuteInput input)
    {
        var (_, appsecret) = AuthorityHelper.AssertDappHeader(_authOptions.CurrentValue, HttpContext,
            input.Key, input.BizData);
        var strategyOutput = _storageProvider.ExecuteThirdPartSecret(input, _alchemyPayAesSignStrategy);
        // strategyOutput.Value = EncryptHelper.AesCbcEncrypt(strategyOutput.Value, appsecret);
        return Task.FromResult(new CommonResponse<CommonThirdPartExecuteOutput>(strategyOutput));
    }

    [HttpPost("thirdPart/alchemySha")]
    public Task<CommonResponse<CommonThirdPartExecuteOutput>> AlchemyShaSignAsync(
        CommonThirdPartExecuteInput input)
    {
        var (_, appsecret) = AuthorityHelper.AssertDappHeader(_authOptions.CurrentValue, HttpContext,
            input.Key, input.BizData);
        var strategyOutput = _storageProvider.ExecuteThirdPartSecret(input, _alchemyPayShaSignStrategy);
        // strategyOutput.Value = EncryptHelper.AesCbcEncrypt(strategyOutput.Value, appsecret);
        return Task.FromResult(new CommonResponse<CommonThirdPartExecuteOutput>(strategyOutput));
    }

    [HttpPost("thirdPart/alchemyHmac")]
    public Task<CommonResponse<CommonThirdPartExecuteOutput>> AlchemyHmacSignAsync(
        CommonThirdPartExecuteInput input)
    {
        var (_, appsecret) = AuthorityHelper.AssertDappHeader(_authOptions.CurrentValue, HttpContext,
            input.Key, input.BizData);
        var strategyOutput = _storageProvider.ExecuteThirdPartSecret(input, _alchemyPayHmacSignStrategy);
        // strategyOutput.Value = EncryptHelper.AesCbcEncrypt(strategyOutput.Value, appsecret);
        return Task.FromResult(new CommonResponse<CommonThirdPartExecuteOutput>(strategyOutput));
    }

    [HttpPost("thirdPart/appleAuth")]
    public Task<CommonResponse<CommonThirdPartExecuteOutput>> AlchemyHmacSignAsync(
        AppleAuthExecuteInput input)
    {
        var (_, appsecret) = AuthorityHelper.AssertDappHeader(_authOptions.CurrentValue, HttpContext,
            input.Key, input.KeyId, input.TeamId, input.ClientId);
        var strategyOutput = _storageProvider.ExecuteThirdPartSecret(input, _appleAuthStrategy);
        // strategyOutput.Value = EncryptHelper.AesCbcEncrypt(strategyOutput.Value, appsecret);
        return Task.FromResult(new CommonResponse<CommonThirdPartExecuteOutput>(strategyOutput));
    }
}