using Chrono.Console.Models;
using Microsoft.EntityFrameworkCore;

namespace Chrono.Console.Database;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Time> Times { get; init; }
    public DbSet<Activity> Activities { get; init; }
}