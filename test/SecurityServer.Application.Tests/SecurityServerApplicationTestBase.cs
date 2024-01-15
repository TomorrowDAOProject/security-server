using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Moq;
using SecurityServer.Options;
using Xunit.Abstractions;

namespace SecurityServer;

public abstract partial class SecurityServerApplicationTestBase : SecurityServerTestBase<SecurityServerApplicationTestModule>
{

    internal new readonly ITestOutputHelper Output;
    protected SecurityServerApplicationTestBase(ITestOutputHelper output) : base(output)
    {
        Output = output;
    }


    internal IOptionsMonitor<AuthorityOptions> MockAuthOptions()
    {
        var authOptions = new AuthorityOptions
        {
            Dapp = new Dictionary<string, string>
            {
                ["caserver"] = "12345678"
            }
        };
        
        var mock = new Mock<IOptionsMonitor<AuthorityOptions>>();
        mock.Setup(s => s.CurrentValue).Returns(authOptions);
        return mock.Object;
    }

    internal IOptionsMonitor<KeyStoreOptions> MockStorageOptions()
    {
        var options = new KeyStoreOptions
        {
            Path = "keys",
            ThirdPart = new Dictionary<string, ThirdPartEncryptOption>
            {
                ["AesGcmData"] = new()
                {
                    Password = "12345678",
                    Mod = "rx"
                },

                ["AesCbcData"] = new()
                {
                    Password = "12345678",
                    Mod = "r"
                },

                ["appleMockKey"] = new()
                {
                    Password = "12345678",
                    Mod = "rx"
                },
                
                ["TestKey"] = new()
                {
                    Password = "12345678",
                    Mod = "x"
                }
            }
        };

        var mock = new Mock<IOptionsMonitor<KeyStoreOptions>>();
        mock.Setup(s => s.CurrentValue).Returns(options);
        return mock.Object;
    }
    
    
    internal IOptionsMonitor<KeyStoreOptions> MockKeyStoreOptions()
    {
        var options = new KeyStoreOptions
        {
            Path = "keys",
            Passwords = new Dictionary<string, string>
            {
                ["4yK5iHxrRYPj7C2JwG938Ss24zq9EMgjgX182S5MJxuN9q32P"] = "12345678"
            }
        };

        var mock = new Mock<IOptionsMonitor<KeyStoreOptions>>();
        mock.Setup(s => s.CurrentValue).Returns(options);
        return mock.Object;
    }
}