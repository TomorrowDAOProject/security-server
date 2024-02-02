using SecurityServer.Common;
using SecurityServer.Dtos;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace SecurityServer.Storage;

public class EncryptTest : SecurityServerApplicationTestBase
{
    
    public EncryptTest(ITestOutputHelper output) : base(output)
    {
    }



    [Fact]
    public void EncryptGcmDtoTest()
    {
        var dto = EncryptDataDto.Builder()
            .Key("123")
            .Information("123")
            .Password("123")
            .RepeatPassword("123", out var success)
            .Secret("123")
            .EncryptType(EncryptType.AesGcm, out var typeSuccess)
            .RandomNonce().Build();
        
        Output.WriteLine(dto.FormatJson());
        Output.WriteLine(dto.TryDecrypt("123", out var secretData, out var message).ToString());
        Output.WriteLine(secretData);
        Output.WriteLine(message);
        secretData.ShouldBe("123");
        
    }   
    
    [Fact]
    public void EncryptCbcDtoTest()
    {
        var dto = EncryptDataDto.Builder()
            .Key("123")
            .Information("123")
            .Password("123")
            .RepeatPassword("123", out var success)
            .Secret("123")
            .EncryptType(EncryptType.AesCbc, out var typeSuccess)
            .RandomNonce().Build();
        
        Output.WriteLine(dto.FormatJson());
        Output.WriteLine(dto.TryDecrypt("123", out var secretData, out var message).ToString());
        Output.WriteLine(secretData);
        Output.WriteLine(message);
        secretData.ShouldBe("123");
        
    }

    [Fact]
    public void EncryptCbc()
    {
        Output.WriteLine(EncryptHelper.AesCbcEncrypt("a", "a"));
        Output.WriteLine(EncryptHelper.AesCbcEncrypt("a", "a"));
    }
    
    
    
}