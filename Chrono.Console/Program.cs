using Chrono.Console.Commands;
using Chrono.Console.Database;
using Chrono.Console.DependencyInjection;
using Chrono.Console.Repositories.Implementations;
using Chrono.Console.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace Chrono.Console;

public static class Program
{
    public static int Main(string[] args)
    {
        var services = new ServiceCollection();
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            var dbPath = Path.Combine(AppContext.BaseDirectory, "chrono.db");
            options.UseSqlite($"Data Source={dbPath}");
        });
        services.AddSingleton<IDatabaseRepository, DatabaseRepository>();
        services.AddSingleton<IActivityRepository, ActivityRepository>();
        services.AddSingleton<ITimeRepository, TimeRepository>();
        
        var registrar = new TypeRegistrar(services);
        var app = new CommandApp(registrar);
        
        app.Configure(config =>
        {
            config.AddCommand<TimeCommand>("time");
            config.AddCommand<DatabaseCommand>("database");
            config.AddCommand<ActivityCommand>("activity");
        });
        
        return app.Run(args);
    }
}