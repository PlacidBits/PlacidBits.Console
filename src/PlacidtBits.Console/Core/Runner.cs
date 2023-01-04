using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace PlacidBits.Console.Core;

public class Runner
{
    private readonly IHostBuilder _hostBuilder;
    
    public Runner(IHostBuilder hostBuilder)
    {
        _hostBuilder = hostBuilder;
    }

    public async Task RunAsync()
    {
        _hostBuilder.ConfigureServices(services =>
            services.AddHostedService<ConsoleHostedService>());
        
        var app = _hostBuilder.Build();

        await app.RunAsync();
    }
}