using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PlacidBits.Console.Core;
using PlacidBits.Console.Core.Parameters;
using PlacidBits.Console.Extensions;

namespace PlacidBits.Console.Tests;

public class RunnerTests
{
    [Fact]
    public async Task BuildRunner_FunctionsHaveDI()
    {
        var runner = RunnerBuilder
            .CreateRunnerBuilder([], default)
            .AddFunction<TestFunction>()
            .AddSqlServer<TestContext>("Dummy connection string")
            .ConfigureServices(services =>
            {
                services.AddTransient<SomeOtherService>();
            })
            .AddCliArgs(["Argument1", "Arg Value 1"])
            .Build();

        await runner.RunAsync();
    }
}

file class TestFunction(
    ILogger<TestFunction> logger,
    CliArguments cliArguments,
    TestContext context,
    SomeOtherService someOtherService)
    : IConsoleFunction
{
    public Task RunAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Hello from the test function!");
        logger.LogInformation("Got cli args {CliArgs}", string.Join(",",cliArguments.Args));
        logger.LogInformation("Got context with connection string: {ConnectionString}", context.Database.GetConnectionString());
        logger.LogInformation("Got a message from my service: {ServiceMessage}", someOtherService.SayHello());
        
        return Task.CompletedTask;
    }
}

file class TestContext(DbContextOptions options) : DbContext(options);

file class SomeOtherService
{
    public string SayHello() => "Hello from the configured service from the function!";
}