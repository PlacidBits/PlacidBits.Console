using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace PlacidBits.Console.Core;

public class Runner
{
    private readonly IHostBuilder _hostBuilder;
    private readonly RunnerType _runnerType;

    public Runner(IHostBuilder hostBuilder, RunnerType runnerType = RunnerType.Default)
    {
        _hostBuilder = hostBuilder;
        _runnerType = runnerType;
    }

    public async Task RunAsync()
    {
        switch (_runnerType)
        {
            case RunnerType.Default:
                _hostBuilder.ConfigureServices(services =>
                    services.AddHostedService<ConsoleHostedService>());
                break;
            case RunnerType.LongRunning:
                _hostBuilder.ConfigureServices(services =>
                    services.AddHostedService<ConsoleBackgroundService>());
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        _hostBuilder.ConfigureServices(services =>
            services.AddHostedService<ConsoleHostedService>());

        var app = _hostBuilder.Build();

        await app.RunAsync();
    }
}