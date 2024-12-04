using Chrono.Console.Database;
using Chrono.Console.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Chrono.Console.Repositories.Implementations;

public sealed class DatabaseRepository(ApplicationDbContext context) : IDatabaseRepository
{
    private const string DatabaseName = "chrono.db";
    
    public bool DatabaseExists()
    {
        var dbPath = Path.Combine(AppContext.BaseDirectory, DatabaseName);
        
        return File.Exists(dbPath);
    }
    
    public void ApplyMigrations()
    {
        context.Database.Migrate();
    }
    
    public bool Delete()
    {
        return context.Database.EnsureDeleted();
    }
}