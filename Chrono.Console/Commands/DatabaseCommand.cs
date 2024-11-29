using Chrono.Console.Database;
using Chrono.Console.Settings;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Chrono.Console.Commands;

internal class DatabaseCommand(ApplicationDbContext dbContext) : Command<DatabaseSettings>
{
    public override int Execute(CommandContext context, DatabaseSettings settings)
    {
        Messages.ShowRule("Banco de dados");
        var select = AnsiConsole.Prompt(new SelectionPrompt<string>()
            .Title("Escolha uma opção:")
            .AddChoices("Status", "Criar", "Deletar", "Sair")
        );
        
        switch (select)
        {
            case "Status":
                CheckDatabaseStatus();
                break;
            case "Criar":
                CreateDatabase();
                break;
            case "Deletar":
                DeleteDatabase();
                break;
            case "Sair":
                Messages.ShowEndProgram();
                return 0;
        }

        return 0;
    }
    
    private static bool DatabaseExists()
    {
        var dbPath = Path.Combine(AppContext.BaseDirectory, "chrono.db");

        if (File.Exists(dbPath)) return true;
        
        Messages.ShowError("Banco de dados [maroon]não[/] existe.");
        return false;
    }
    
    private static void CheckDatabaseStatus()
    {
        if (DatabaseExists())
        {
            Messages.ShowSuccess("Banco de dados [green]online[/].");
        }
    }
    
    private void CreateDatabase()
    {
        try
        {
            dbContext.Database.Migrate();
            Messages.ShowSuccess("Banco de dados [green]criado[/] e [green]migrações aplicadas[/].");
        }
        catch (Exception ex)
        {
            Messages.ShowError("Banco de dados [maroon]não[/] foi criado.");
            System.Console.WriteLine(ex.Message);
        }
    }
    
    private void DeleteDatabase()
    {
        var confirmation =
            AnsiConsole.Prompt(new ConfirmationPrompt("Tem certeza que deseja [maroon]excluir[/] o banco de dados?"));
        
        if (!confirmation) 
        {
            Messages.ShowCancelOperation();
            return;
        }
        
        try
        {
            if (!DatabaseExists()) return;
            
            var dbDeleted = dbContext.Database.EnsureDeleted();
            if (dbDeleted)
            {
                Messages.ShowSuccess("Banco de dados [maroon]excluído[/] com [green]sucesso[/].");
            } else {
                Messages.ShowError("Banco de dados [maroon]não[/] foi deletado.");
            }
        }
        catch (Exception ex)
        {
            Messages.ShowError("Problema no banco de dados ao tentar [maroon]excluir[/].");
            System.Console.WriteLine(ex.Message);
        }
    }
}