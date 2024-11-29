using Chrono.Console.Repositories.Interfaces;
using Chrono.Console.Settings;
using Spectre.Console.Cli;

namespace Chrono.Console.Commands;

internal class ActivityCommand(IActivityRepository activityRepository) : Command<ActivitySettings>
{
    public override int Execute(CommandContext context, ActivitySettings settings)
    {
        throw new NotImplementedException();
    }
}