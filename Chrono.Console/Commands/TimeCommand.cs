using Chrono.Console.Repositories.Interfaces;
using Chrono.Console.Settings;
using Spectre.Console.Cli;

namespace Chrono.Console.Commands;

internal class TimeCommand(ITimeRepository timeRepository) : Command<TimeSettings>
{
    public override int Execute(CommandContext context, TimeSettings settings)
    {
        throw new NotImplementedException();
    }
}