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
            .CreateRunnerBuilder(default)
            .AddFunction<TestFunction>()
            .AddSqlServer<TestContext>("Dummy connection string")
            .ConfigureServices(services =>
            {
                services.AddTransient<SomeOtherService>();
            })
            .AddCliArgs(new[] { "Argument1", "Arg Value 1" })
            .Build();

        await runner.RunAsync();
    }
}

file class TestFunction : IConsoleFunction
{
    private readonly ILogger<TestFunction> _logger;
    private readonly CliArguments _cliArguments;
    private readonly TestContext _context;
    private readonly SomeOtherService _someOtherService;

    public TestFunction(ILogger<TestFunction> logger, CliArguments cliArguments, TestContext context, SomeOtherService someOtherService)
    {
        _logger = logger;
        _cliArguments = cliArguments;
        _context = context;
        _someOtherService = someOtherService;
    }

    public Task RunAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Hello from the test function!");
        _logger.LogInformation("Got cli args {CliArgs}", string.Join(",",_cliArguments.Args));
        _logger.LogInformation("Got context with connection string: {ConnectionString}", _context.Database.GetConnectionString());
        _logger.LogInformation("Got a message from my service: {ServiceMessage}", _someOtherService.SayHello());
        
        return Task.CompletedTask;
    }
}

file class TestContext : DbContext
{
    public TestContext(DbContextOptions options) : base(options)
    {
        
    }
}

file class SomeOtherService
{
    public string SayHello() => "Hello from the configured service from the function!";
}