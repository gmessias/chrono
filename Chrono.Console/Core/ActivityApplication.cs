using Chrono.Console.Enums;
using Chrono.Console.Models;
using Chrono.Console.Repositories.Interfaces;
using Spectre.Console;

namespace Chrono.Console.Core;

public sealed class ActivityApplication(IActivityRepository activityRepository)
{
    public void Views()
    {
        var select = AnsiConsole.Prompt(new SelectionPrompt<EnumActivityViewsOptions>()
            .Title("Escolha uma opção de visualização:")
            .AddChoices(Enum.GetValues<EnumActivityViewsOptions>())
            .UseConverter(option => option.GetDescription())
        );
        
        Activity? activity;
        switch (select)
        {
            case EnumActivityViewsOptions.ListarTodos:
                ActivityTable(activityRepository.GetAll());
                break;
            case EnumActivityViewsOptions.ListarTodosAtivos:
                ActivityTable(activityRepository.GetAllActive());
                break;
            case EnumActivityViewsOptions.ListarPorFiltro:
                ActivityTable(activityRepository.GetLikeName(AskActivityName()));
                break;
            case EnumActivityViewsOptions.BuscarPorCodigo:
                activity = SearchActivity(AskActivityId());
                if (activity != null)
                {
                    ActivityTable(activity);
                }
                break;
            case EnumActivityViewsOptions.BuscarPorNome:
                activity = SearchActivity(AskActivityName());
                if (activity != null)
                {
                    ActivityTable(activity);
                }
                break;
            case EnumActivityViewsOptions.Sair:
                Messages.ShowEndProgram();
                return;
        }
    }
    
    public void Create()
    {
        var name = AskActivityName();
        var nameExist = activityRepository.NameExists(name);
        if (nameExist)
        {
            Messages.ShowWarning("Este nome [maroon]já existe[/].");
            return;
        }
        
        var activity = new Activity { Name = name };
        try
        {
            activityRepository.Create(activity);
            Messages.ShowSuccess($"Atividade [navy]{activity.Name}[/] criada com [green]sucesso[/].");
        }
        catch (Exception ex)
        {
            Messages.ShowError("Atividade [maroon]não[/] foi criado.");
            System.Console.WriteLine(ex.Message);
        }
    }
    
    public void Update()
    {
        var id = AskActivityId();
        var activity = SearchActivity(id);
        if (activity != null)
        {
            AnsiConsole.MarkupLine($"Id: {activity.Id}, Nome: {activity.Name}, Ativo: {activity.IsActive}");
            var confirmationName = AnsiConsole.Prompt(new ConfirmationPrompt("Deseja atualizar o [navy]nome[/]?"));
            if (confirmationName)
            {
                var newName = AskActivityName();
                var nameExist = activityRepository.NameExists(newName);
                if (nameExist)
                {
                    Messages.ShowWarning("Este nome [maroon]já existe[/].");
                    return;
                }
                activity.Name = newName;
            }
        
            var confirmationIsActive = AnsiConsole.Prompt(new ConfirmationPrompt("Deseja atualizar o [navy]status[/] da atividade?"));
            if (confirmationIsActive)
            {
                var newIsActive = AnsiConsole.Prompt(new SelectionPrompt<EnumActivityActive>()
                    .Title("Informe o novo [navy]status[/] da atividade:")
                    .AddChoices(Enum.GetValues<EnumActivityActive>())
                );
                switch (newIsActive)
                {
                    case EnumActivityActive.Ativo:
                        activity.IsActive = true;
                        break;
                    case EnumActivityActive.Inativo:
                        activity.IsActive = false;
                        break;
                    case EnumActivityActive.Cancelar:
                        Messages.ShowEndProgram();
                        return;
                }
            }
        } else {
            return;
        }
        
        try
        {
            activityRepository.Update(activity);
            Messages.ShowSuccess($"Atividade [navy]{activity.Name}[/] atualizada com [green]sucesso[/].");
        }
        catch (Exception ex)
        {
            Messages.ShowError("Atividade [maroon]não[/] foi atualizada.");
            System.Console.WriteLine(ex.Message);
        }
    }
    
    public void Delete()
    {
        var id = AskActivityId();
        var activity = SearchActivity(id);
        if (activity != null)
        {
            AnsiConsole.MarkupLine($"Id: {activity.Id}, Nome: {activity.Name}, Ativo: {activity.IsActive}");
            var confirmation = AnsiConsole.Prompt(new ConfirmationPrompt("Deseja [red]excluir permanentemente[/] essa atividade?"));
            if (confirmation) 
            {
                try 
                {
                    activityRepository.Delete(activity);
                    Messages.ShowSuccess($"Atividade [navy]{activity.Name}[/] de código [navy]{activity.Id}[/] deletada com [green]sucesso[/].");
                } 
                catch (Exception ex)
                {
                    Messages.ShowError("Atividade [maroon]não[/] foi deletada.");
                    System.Console.WriteLine(ex.Message);
                }
            }
        }
    }
    
    private Activity? SearchActivity(long id)
    {
        var activity = activityRepository.GetById(id);
        if (activity is null)
        {
            Messages.ShowError("Atividade [maroon]não[/] foi encontrada.");
        }
        return activity;
    }
    
    private Activity? SearchActivity(string name)
    {
        var activity = activityRepository.GetByName(name);
        if (activity is null)
        {
            Messages.ShowError("Atividade [maroon]não[/] foi encontrada.");
        }
        return activity;
    }
    
    public long AskActivityId()
    {
        return AnsiConsole.Prompt(new TextPrompt<long>("Informe o [navy]código[/] da atividade:")
            .Validate(input => input > 0 
                ? ValidationResult.Success() 
                : ValidationResult.Error("[red]Código deve ser maior que zero.[/]"))
        );
    }
    
    public string AskActivityName()
    {
        return AnsiConsole.Prompt(new TextPrompt<string>("Informe o [navy]nome[/] da atividade:")
            .Validate(input => 
            {
                if (string.IsNullOrWhiteSpace(input))
                {
                    return ValidationResult.Error("[red]O nome deve ser preenchido.[/]");
                }
                if (input.Length > 16)
                {
                    return ValidationResult.Error("[red]O nome da atividade não pode ter mais de 16 caracteres.[/]");
                }
                return ValidationResult.Success();
            })
        );
    }
    
    private void ActivityTable(Activity activity)
    {
        var activities = new List<Activity> { activity };
        ActivityTable(activities);
    }
    
    public void ActivityTable(IEnumerable<Activity> activities)
    {
        var table = new Table();
        table.AddColumn(new TableColumn("Id").Centered());
        table.AddColumn(new TableColumn("Atividade").Centered());
        table.AddColumn(new TableColumn("Ativo").Centered());
        foreach (var activity in activities)
        {
            table.AddRow(
                activity.Id.ToString(), 
                activity.Name, 
                activity.IsActive.ToString());
        }
        AnsiConsole.Write(table);
    }
}