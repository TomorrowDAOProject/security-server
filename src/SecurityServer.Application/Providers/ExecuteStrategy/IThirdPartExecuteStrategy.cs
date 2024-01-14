using SecurityServer.Common;
using SecurityServer.Dtos;

namespace SecurityServer.Providers.ExecuteStrategy;

public interface IThirdPartExecuteStrategy<in TInput, out TOutput> where TInput : BaseThirdPartExecuteInput
{
    public ThirdPartExecuteStrategy ExecuteStrategy();

    public TOutput Execute(string secret, TInput input);

}