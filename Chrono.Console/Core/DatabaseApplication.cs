using Chrono.Console.Repositories.Interfaces;
using Spectre.Console;

namespace Chrono.Console.Core;

public sealed class DatabaseApplication(IDatabaseRepository databaseRepository)
{
    public void Status()
    {
        var databaseExists = databaseRepository.DatabaseExists();
        if (databaseExists)
        {
            Messages.ShowSuccess("Banco de dados [green]online[/].");
        } else {
            Messages.ShowError("Banco de dados [maroon]não[/] existe.");
        }
    }
    
    public void Create()
    {
        var databaseExists = databaseRepository.DatabaseExists();
        if (databaseExists)
        {
            Messages.ShowWarning("Banco de dados [green]já está criado[/].");
            return;
        }
        
        try 
        {
            databaseRepository.ApplyMigrations();
            Messages.ShowSuccess("Banco de dados [green]criado[/] e [green]migrações aplicadas[/].");
        }
        catch (Exception ex)
        {
            Messages.ShowError("Banco de dados [maroon]não[/] foi criado.");
            System.Console.WriteLine(ex.Message);
        }
    }
    
    public void Delete()
    {
        var confirm = AnsiConsole.Prompt(new ConfirmationPrompt("Tem certeza que deseja [maroon]excluir[/] o banco de dados?"));
        if (!confirm) 
        {
            Messages.ShowCancelOperation();
            return;
        }
        
        try
        {
            var databaseExists = databaseRepository.DatabaseExists();
            if (databaseExists)
            {
                var deleted = databaseRepository.Delete();
                if (deleted)
                {
                    Messages.ShowSuccess("Banco de dados [maroon]excluído[/] com [green]sucesso[/].");
                } else {
                    Messages.ShowError("Banco de dados [maroon]não[/] foi deletado.");
                }
            } else {
                Messages.ShowError("Banco de dados [maroon]não[/] existe.");
            }
        }
        catch (Exception ex)
        {
            Messages.ShowError("Problema no banco de dados ao tentar [maroon]excluir[/].");
            System.Console.WriteLine(ex.Message);
        }
    }
}