using System.Security.Cryptography;
using System.Text;
using SecurityServer.Common;
using SecurityServer.Dtos;

namespace SecurityServer.Providers.ExecuteStrategy;

public class AlchemyPayShaSignStrategy : IThirdPartExecuteStrategy<CommonThirdPartExecuteInput, CommonThirdPartExecuteOutput>
{
    
    public ThirdPartExecuteStrategy ExecuteStrategy()
    {
        return ThirdPartExecuteStrategy.AlchemyPaySha1;
    }

    public CommonThirdPartExecuteOutput Execute(string secret, CommonThirdPartExecuteInput input)
    {
        var source = string.Join(CommonConstant.EmptyString, input.Key, secret, input.BizData);
        var bytes = Encoding.UTF8.GetBytes(source);
        var hashBytes = SHA1.Create().ComputeHash(bytes);

        var sb = new StringBuilder();
        foreach (var t in hashBytes)
        {
            sb.Append(t.ToString("X2"));
        }
        var result = sb.ToString().ToLower();
        return new CommonThirdPartExecuteOutput(result);
    }
    
}