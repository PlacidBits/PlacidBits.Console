using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace PlacidBits.Console.Core;

public class ConsoleBackgroundService : BackgroundService
{
    private readonly IEnumerable<IConsoleFunction> _functions;
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly ILogger<ConsoleBackgroundService> _logger;

    public ConsoleBackgroundService(IEnumerable<IConsoleFunction> functions, ILogger<ConsoleBackgroundService> logger, IHostApplicationLifetime applicationLifetime)
    {
        _functions = functions;
        _logger = logger;
        _applicationLifetime = applicationLifetime;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_functions.Any())
                {
                    _logger.LogInformation("Running functions");
                    await Task.WhenAll(_functions.Select(async f => await f.RunAsync(stoppingToken)));
                }
            }
        }
        catch (TaskCanceledException)
        {
        }
        catch
        {
            Environment.Exit(1);
        }
    }
}