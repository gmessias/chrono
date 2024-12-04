using Chrono.Console.Enums;
using Chrono.Console.Repositories.Interfaces;
using Chrono.Console.Settings;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Chrono.Console.Commands;

internal class TimeCommand(ITimeRepository timeRepository) : Command<TimeSettings>
{
    public override int Execute(CommandContext context, TimeSettings settings)
    {
        var select = AnsiConsole.Prompt(new SelectionPrompt<EnumTimeOptions>()
            .Title("Escolha uma opção:")
            .AddChoices(Enum.GetValues<EnumTimeOptions>())
        );

        switch (select)
        {
            case EnumTimeOptions.Visualizar:
                timeRepository.Views();
                break;
            case EnumTimeOptions.Inserir:
                timeRepository.Create();
                break;
            case EnumTimeOptions.Editar:
                timeRepository.Update();
                break;
            case EnumTimeOptions.Deletar:
                timeRepository.Delete();
                break;
            case EnumTimeOptions.Sair:
                Messages.ShowEndProgram();
                return 0;
        }
        
        return 0;
    }
}