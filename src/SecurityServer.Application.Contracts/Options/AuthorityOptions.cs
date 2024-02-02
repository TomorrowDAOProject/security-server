using System.Collections.Generic;

namespace SecurityServer.Options;

public class AuthorityOptions
{
    
    // appid => appSecret
    public Dictionary<string, string> Dapp { get; set; } = new();
    
    
}