using System;
using System.Security.Cryptography;
using System.Text;
using SecurityServer.Common;
using SecurityServer.Dtos;

namespace SecurityServer.Providers.ExecuteStrategy;

public class AlchemyPayHmacSignStrategy : IThirdPartExecuteStrategy<CommonThirdPartExecuteInput, CommonThirdPartExecuteOutput>
{
    
    public ThirdPartExecuteStrategy ExecuteStrategy()
    {
        return ThirdPartExecuteStrategy.AlchemyPayHmac;
    }

    public CommonThirdPartExecuteOutput Execute(string secret, CommonThirdPartExecuteInput input)
    {
        using var hmac = new HMACSHA1(Encoding.UTF8.GetBytes(secret));
        var achSign = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(input.BizData)));
        return new CommonThirdPartExecuteOutput(achSign);
    }
    
}