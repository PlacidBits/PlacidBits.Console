using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlacidBits.Console.Core.Parameters;
using Serilog;

namespace PlacidBits.Console.Core;

public class RunnerBuilder(IHostBuilder hostBuilder, RunnerType runnerType)
{
    public static RunnerBuilder CreateRunnerBuilder(
        string[] args,
        LoggerConfiguration? loggerConfiguration = null,
        RunnerType runnerType = RunnerType.Default)
    {
        var logConfig = loggerConfiguration ?? new LoggerConfiguration().WriteTo.Console();
        Log.Logger = logConfig.CreateLogger();

        return new RunnerBuilder(Host.CreateDefaultBuilder(args).UseSerilog(), runnerType);
    }

    public IHostBuilder HostBuilder => hostBuilder;

    public Runner Build() => new Runner(hostBuilder, runnerType);

    public RunnerBuilder ConfigureServices(Action<IServiceCollection> configureDelegate)
    {
        hostBuilder.ConfigureServices(configureDelegate);

        return this;
    }

    public RunnerBuilder AddFunction<TFunction>()
        where TFunction : class, IConsoleFunction
    {
        hostBuilder.ConfigureServices(services => services.AddScoped<IConsoleFunction, TFunction>());

        return this;
    }

    public RunnerBuilder AddCliArgs(string[] args)
    {
        hostBuilder.ConfigureServices(services => services.AddSingleton(new CliArguments(args)));

        return this;
    }
}