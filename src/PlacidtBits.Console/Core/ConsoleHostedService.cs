using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace PlacidBits.Console.Core;

public class ConsoleHostedService : IHostedService
{
    private readonly IEnumerable<IConsoleFunction> _functions;
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly ILogger<ConsoleHostedService> _logger;

    public ConsoleHostedService(
        IEnumerable<IConsoleFunction> functions,
        IHostApplicationLifetime applicationLifetime,
        ILogger<ConsoleHostedService> logger)
    {
        _functions = functions;
        _applicationLifetime = applicationLifetime;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _applicationLifetime.ApplicationStarted.Register(() =>
        {
            Task.Run(async () =>
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        _logger.LogInformation("Running functions");
                        await Task.WhenAll(_functions.Select(async f => await f.RunAsync(cancellationToken)));
                    }
                    catch (Exception ex) when (False(() => _logger.LogCritical(ex, "Fatal error")))
                    {
                        throw;
                    }
                    finally
                    {
                        await Log.CloseAndFlushAsync();
                        _logger.LogInformation("Run completed. Stopping application");
                        _applicationLifetime.StopApplication();
                    }
                }
            }, cancellationToken);
        });
    }

    private static bool False(Action action) { action(); return false; }
    
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}