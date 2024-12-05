using Chrono.Console.Core;
using Chrono.Console.Enums;
using Chrono.Console.Repositories.Interfaces;
using Chrono.Console.Settings;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Chrono.Console.Commands;

internal class ActivityCommand(IActivityRepository activityRepository) : Command<ActivitySettings>
{
    public override int Execute(CommandContext context, ActivitySettings settings)
    {
        Messages.ShowRule("Atividades");
        
        var select = AnsiConsole.Prompt(new SelectionPrompt<EnumActivityOptions>()
            .Title("Escolha uma opção:")
            .AddChoices(Enum.GetValues<EnumActivityOptions>())
        );

        var activityApplication = new ActivityApplication(activityRepository);
        
        switch (select)
        {
            case EnumActivityOptions.Visualizar:
                activityApplication.Views();
                break;
            case EnumActivityOptions.Inserir:
                activityApplication.Create();
                break;
            case EnumActivityOptions.Editar:
                activityApplication.Update();
                break;
            case EnumActivityOptions.Deletar:
                activityApplication.Delete();
                break;
            case EnumActivityOptions.Sair:
                Messages.ShowEndProgram();
                return 0;
        }

        return 0;
    }
}