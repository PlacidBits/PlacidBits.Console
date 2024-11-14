﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace PlacidBits.Console.Core;

public class Runner(IHostBuilder hostBuilder)
{
    public async Task RunAsync()
    {
        hostBuilder.ConfigureServices(services =>
            services.AddHostedService<ConsoleHostedService>());
        
        var app = hostBuilder.Build();

        await app.RunAsync();
    }
}