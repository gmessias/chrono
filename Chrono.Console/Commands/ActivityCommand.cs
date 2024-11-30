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
                View();
                break;
            case "Inserir":
                Insert();
                break;
            case "Editar":
                Edit();
                break;
            case "Deletar":
                Delete();
                break;
            case "Sair":
                Messages.ShowEndProgram();
                return 0;
        }

        return 0;
    }
    
    private void View()
    {
        Messages.ShowRule("Visualização de atividades");
        var select = AnsiConsole.Prompt(new SelectionPrompt<string>()
            .Title("Escolha uma opção de visualização:")
            .AddChoices("Listar todos", "Buscar por código", "Sair")
        );
        
        switch (select)
        {
            case "Listar todos":
                var activities = activityRepository.GetAll();
                activityRepository.ActivityTable(activities);
                break;
            case "Buscar por código":
                var id = AnsiConsole.Prompt(new TextPrompt<long>("Informe o [navy]código[/] da atividade:")
                    .Validate(input => input > 0 
                        ? ValidationResult.Success() 
                        : ValidationResult.Error("[red]Código deve ser maior que zero.[/]"))
                );
                var activity = activityRepository.GetById(id);
                if (activity is null)
                {
                    Messages.ShowError("Atividade [maroon]não[/] foi encontrada.");
                    return;
                }
                activityRepository.ActivityTable(activity);
                break;
            case "Sair":
                Messages.ShowEndProgram();
                return;
        }
    }
    
    private void Insert()
    {
        activityRepository.Create();
    }
    
    private void Edit()
    {
    }
    
    private void Delete()
    {
    }
}