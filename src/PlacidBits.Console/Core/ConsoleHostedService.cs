using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace PlacidBits.Console.Core;

public class ConsoleHostedService(
    IHostApplicationLifetime applicationLifetime,
    ILogger<ConsoleHostedService> logger,
    IServiceProvider serviceProvider)
    : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        applicationLifetime.ApplicationStarted.Register(() =>
        {
            Task.Run(async () =>
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        await using var serviceScope = serviceProvider.CreateAsyncScope();
                        var functions = serviceScope.ServiceProvider.GetServices<IConsoleFunction>();
                        
                        logger.LogInformation("Running functions");
                        await Task.WhenAll(functions.Select(async f => await f.RunAsync(cancellationToken)));
                    }
                    catch (Exception ex) when (False(() => logger.LogCritical(ex, "Fatal error")))
                    {
                        throw;
                    }
                    finally
                    {
                        await Log.CloseAndFlushAsync();
                        logger.LogInformation("Run completed. Stopping application");
                        applicationLifetime.StopApplication();
                    }
                }
            }, cancellationToken);
        });
        return Task.CompletedTask;
    }

    private static bool False(Action action) { action(); return false; }
    
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}