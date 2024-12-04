using Chrono.Console.Core;
using Chrono.Console.Enums;
using Chrono.Console.Repositories.Interfaces;
using Chrono.Console.Settings;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Chrono.Console.Commands;

public sealed class DatabaseCommand(IDatabaseRepository databaseRepository) : Command<DatabaseSettings>
{
    public override int Execute(CommandContext context, DatabaseSettings settings)
    {
        Messages.ShowRule("Banco de dados");
        
        var select = AnsiConsole.Prompt(new SelectionPrompt<EnumDatabaseOptions>()
            .Title("Escolha uma opção:")
            .AddChoices(Enum.GetValues<EnumDatabaseOptions>())
        );

        var databaseApplication = new DatabaseApplication(databaseRepository);
        
        switch (select)
        {
            case EnumDatabaseOptions.Status:
                databaseApplication.Status();
                break;
            case EnumDatabaseOptions.Criar:
                databaseApplication.Create();
                break;
            case EnumDatabaseOptions.Deletar:
                databaseApplication.Delete();
                break;
            case EnumDatabaseOptions.Sair:
                Messages.ShowEndProgram();
                return 0;
        }

        return 0;
    }
}