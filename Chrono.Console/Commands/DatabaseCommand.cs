using Chrono.Console.Database;
using Chrono.Console.Settings;
using Spectre.Console.Cli;

namespace Chrono.Console.Commands;

internal class DatabaseCommand(ApplicationDbContext dbContext) : Command<DatabaseSettings>
{
    public override int Execute(CommandContext context, DatabaseSettings settings)
    {
        throw new NotImplementedException();
    }
}