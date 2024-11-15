# PlacidBits.Console
Console app starter package

# Installation

- Create a .NET 9-based console application.
- Install the `PlacidBits.Console` package. You may optionally install the `PlacidBits.Console.Extensions` package

# Setup

## Creating the RunnerBuilder
This library follows the familiar applicaiton builder pattern made popular by .NET Core 1.0. In a similar way, you start the application by creating a `RunnerBuilder` and then adding features to it.

```csharp
using PlacidBits.Console;

var builder = RunnerBuilder.CreateRunnerBuilder(args);
```

The constructor arguments are as follows:
- `args` - This should be the args passed into the program from the command line. In a .NET5 or below solution, this would be the args passed into the `public static void Main(string[] args)` function. In a .NET7+ application, it will be the same args, just available globally as the `args` variable. This argument gets passed to the internal call to `Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()` call and can be used to accomplish everything that it would in a scenario dealing directly with the `HostBuilder`.

## Adding Functions

In order to get your own code to run, you need to create a Function. Do this by creating a class that implements the `PlacidBits.Console.Core.IConsoleFunction` interface:

```csharp
class MyFunction : IConsoleFunction
{
    public Task RunAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
```

Here, you can also inject dependencies from the DI container:

```csharp
class MyFunction : IConsoleFunction
{
    private readonly ILogger<MyFunction> _logger;

    // Grab a logger from DI 
    public MyFunction(ILogger<MyFunction> logger)
    {
        _logger = logger;
    }

    public Task RunAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Running my test function!");

        return Task.CompletedTask;
    }
}
```

When you have your Function created, add it to your `RunnerBuilder` by using the `AddFunction<TFunction>()` generic method:

```csharp
using PlacidBits.Console;

var builder = RunnerBuilder.CreateRunnerBuilder(args);

builder.AddFunction<MyFunction>();
```

## Adding Other Services

Once you've created your builder, you'll have console Logging available by default via Serilog. You can then opt in to any of the built-in supported services (mentioned above) available.

For example, if you've opted to install the `PlacidBits.Console.Extensions` package, you can add an EF Core `DbContext` and connect it to MS Sql Server with the following:

```csharp
using PlacidBits.Console;
using PlacidBits.Console.Extensions;

var builder = RunnerBuilder.CreateRunnerBuilder(args);

builder.AddFunction<MyFunction>();

builder.AddSqlServer<MyContext>("myConnectionStringValue");
```

You can also use the `ConfigureServices` method to add your own services to the dependency injection container:

```csharp
using PlacidBits.Console;

var builder = RunnerBuilder.CreateRunnerBuilder(args);

builder.AddFunction<MyFunction>();

builder.ConfigureServices(services => {
    services.AddTransient<MyExampleService>();
});
```

You also have direct read-only access to the internal `IHostBuilder` to override any specific configurations on the host:

```csharp
using PlacidBits.Console;

var builder = RunnerBuilder.CreateRunnerBuilder(args);

builder.AddFunction<MyFunction>();

builder.HostBuilder.ConfigureLogging(logger => {
    // example logging config override code
});
```

# Usage

Once you've set up your `RunnerBuilder` with all of the required services and at least one Function that defines the work you want this console app to do, you can build and run it:

```csharp
using PlacidBits.Console;

var builder = RunnerBuilder.CreateRunnerBuilder(args);

builder.AddFunction<MyFunction>();

var app = builder.Build();

await app.RunAsync();
```

Creating of the runner can be done in one line for succintness:

```csharp
using PlacidBits.Console;

var app = RunnerBuilder.CreateRunnerBuilder(args)
    .AddFunction<MyFunction>()
    .AddSqlServer<MyContext>("connectionStringValue")
    .Build();

await app.RunAsync();
```

# Testing

Since the Function that defines your console app's work is done in a completely separate class, you can unit test that class and inject mocks into the constructor. See the test project for examples.
