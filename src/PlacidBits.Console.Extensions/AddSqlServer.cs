using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PlacidBits.Console.Core;

namespace PlacidBits.Console.Extensions;

public static class DataAccessExtensions
{
    public static RunnerBuilder AddSqlServer<TContext>(
        this RunnerBuilder runnerBuilder,
        string connectionString) where TContext : DbContext
    {
        runnerBuilder.ConfigureServices(services =>
            services.AddDbContext<TContext>(options =>
                options.UseSqlServer(connectionString)));
        return runnerBuilder;
    }
}