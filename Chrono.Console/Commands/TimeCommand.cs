using Chrono.Console.Core;
using Chrono.Console.Enums;
using Chrono.Console.Repositories.Interfaces;
using Chrono.Console.Settings;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Chrono.Console.Commands;

internal class TimeCommand(ITimeRepository timeRepository, IActivityRepository activityRepository) : Command<TimeSettings>
{
    public override int Execute(CommandContext context, TimeSettings settings)
    {
        Messages.ShowRule("Marcação de horas");
        
        var select = AnsiConsole.Prompt(new SelectionPrompt<EnumTimeOptions>()
            .Title("Escolha uma opção:")
            .AddChoices(Enum.GetValues<EnumTimeOptions>())
        );
        
        var timeApplication = new TimeApplication(timeRepository, activityRepository);

        switch (select)
        {
            case EnumTimeOptions.Visualizar:
                timeApplication.Views();
                break;
            case EnumTimeOptions.Inserir:
                timeApplication.Create();
                break;
            case EnumTimeOptions.Editar:
                timeApplication.Update();
                break;
            case EnumTimeOptions.Deletar:
                timeApplication.Delete();
                break;
            case EnumTimeOptions.Sair:
                Messages.ShowEndProgram();
                return 0;
        }
        
        return 0;
    }
}