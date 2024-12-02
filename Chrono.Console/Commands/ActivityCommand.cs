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
        var select = AnsiConsole.Prompt(new SelectionPrompt<string>()
            .Title("Escolha uma opção:")
            .AddChoices("Visualizar", "Inserir", "Editar", "Deletar", "Sair")
        );
        
        switch (select)
        {
            case "Visualizar":
                activityRepository.Views();
                break;
            case "Inserir":
                activityRepository.Create();
                break;
            case "Editar":
                activityRepository.Update();
                break;
            case "Deletar":
                activityRepository.Delete();
                break;
            case "Sair":
                Messages.ShowEndProgram();
                return 0;
        }

        return 0;
    }
}