using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace PlacidBits.Console.Core;

public class Runner(IHostBuilder hostBuilder, RunnerType runnerType = RunnerType.Default)
{
    public async Task RunAsync()
    {
        switch (runnerType)
        {
            case RunnerType.Default:
                hostBuilder.ConfigureServices(services =>
                    services.AddHostedService<ConsoleHostedService>());
                break;
            case RunnerType.LongRunning:
                hostBuilder.ConfigureServices(services =>
                    services.AddHostedService<ConsoleBackgroundService>());
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        var app = hostBuilder.Build();

        await app.RunAsync();
    }
}