using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlacidBits.Console.Core.Parameters;
using Serilog;
using Serilog.Events;

namespace PlacidBits.Console.Core;

public class RunnerBuilder
{
    private readonly RunnerType _runnerType;

    public static RunnerBuilder CreateRunnerBuilder(
        string[] args,
        LoggerConfiguration? loggerConfiguration = null,
        RunnerType runnerType = RunnerType.Default)
    {
        var logConfig = loggerConfiguration ?? new LoggerConfiguration().WriteTo.Console();
        Log.Logger = logConfig.CreateLogger();

        return new RunnerBuilder(Host.CreateDefaultBuilder(args).UseSerilog(), runnerType);
    }

    public IHostBuilder HostBuilder => _hostBuilder;

    private readonly IHostBuilder _hostBuilder;

    public RunnerBuilder(IHostBuilder hostBuilder, RunnerType runnerType)
    {
        _hostBuilder = hostBuilder;
        _runnerType = runnerType;
    }

    public Runner Build() => new Runner(_hostBuilder, _runnerType);

    public RunnerBuilder ConfigureServices(Action<IServiceCollection> configureDelegate)
    {
        _hostBuilder.ConfigureServices(configureDelegate);

        return this;
    }

    public RunnerBuilder AddFunction<TFunction>()
        where TFunction : class, IConsoleFunction
    {
        _hostBuilder.ConfigureServices(services => services.AddTransient<IConsoleFunction, TFunction>());

        return this;
    }

    public RunnerBuilder AddCliArgs(string[] args)
    {
        _hostBuilder.ConfigureServices(services => services.AddSingleton(new CliArguments(args)));

        return this;
    }
}