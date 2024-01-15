using System;
using System.Security.Authentication;
using Microsoft.AspNetCore.Http;
using SecurityServer.Options;

namespace SecurityServer.Common;

public class AuthorityHelper
{
    public static Tuple<string, string> AssertDappHeader(AuthorityOptions authorityOptions, HttpContext context,
        params string[] values)
    {
        var appid = context.Request.Headers["appid"].ToString();
        if (appid.IsNullOrEmpty())
            throw new AuthenticationException("AppId missing");

        var signature = context.Request.Headers["signature"].ToString();
        if (signature.IsNullOrEmpty())
            throw new AuthenticationException("Signature missing");

        if (!authorityOptions.Dapp.TryGetValue(appid, out var appSecret))
            throw new AuthenticationException("Invalid AppId");

        AssertSignature(signature, appSecret, values);
        return Tuple.Create(appid, appSecret);
    }

    public static void AssertSignature(string signature, string secret, params string[] values)
    {
        var signSource = string.Join("", values);
        var expectSignature = CalculateSignature(signSource, secret);
        if (signature != expectSignature) throw new AuthenticationException("Dapp signature verification failed");
    }

    public static string CalculateSignature(string source, string secret)
    {
        return EncryptHelper.AesCbcEncrypt(source, secret);
    }
}