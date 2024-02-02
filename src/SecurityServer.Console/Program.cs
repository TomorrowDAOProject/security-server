using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SecurityServer.Command;

namespace SecurityServer;

public class Program
{
    public static Task<int> Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
        
        new CommandProvider(configuration).Start();
        return Task.FromResult(0);
    }
}