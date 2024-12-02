using Chrono.Console.Database;
using Chrono.Console.Models;
using Chrono.Console.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;

namespace Chrono.Console.Repositories.Implementations;

public sealed class ActivityRepository(ApplicationDbContext context) : IActivityRepository
{
    public IEnumerable<Activity> GetAll()
    {
        return context.Activities
            .AsNoTracking()
            .ToList();
    }
    
    public IEnumerable<Activity> GetAllActive()
    {
        return context.Activities
            .AsNoTracking()
            .Where(x => x.IsActive)
            .ToList();
    }
    
    public Activity? GetById(long id)
    {
        return context.Activities
            .FirstOrDefault(x => x.Id == id);
    }
    
    public Activity? GetByName(string name)
    {
        return context.Activities
            .FirstOrDefault(x => x.Name.ToLower() == name.ToLower());
    }
    
    public IEnumerable<Activity> GetLikeName(string name)
    {
        return context.Activities
            .AsNoTracking()
            .Where(x => EF.Functions.Like(x.Name.ToLower(), $"%{name.ToLower()}%"))
            .ToList();
    }
    
    public void Views()
    {
        Messages.ShowRule("Visualização de atividades");
        var select = AnsiConsole.Prompt(new SelectionPrompt<string>()
            .Title("Escolha uma opção de visualização:")
            .AddChoices("Listar todos", "Listar todos ativos", "Listar por filtro", "Buscar por código", "Buscar por nome", "Sair")
        );
        
        switch (select)
        {
            case "Listar todos":
                ActivityTable(GetAll());
                break;
            case "Listar todos ativos":
                ActivityTable(GetAllActive());
                break;
            case "Listar por filtro":
                ActivityTable(GetLikeName(AskActivityName()));
                break;
            case "Buscar por código":
                var activity = GetById(AskActivityId());
                if (activity is null)
                {
                    Messages.ShowError("Atividade [maroon]não[/] foi encontrada.");
                    return;
                }
                ActivityTable(activity);
                break;
            case "Buscar por nome":
                var activityByName = GetByName(AskActivityName());
                if (activityByName is null)
                {
                    Messages.ShowError("Atividade [maroon]não[/] foi encontrada.");
                    return;
                }
                ActivityTable(activityByName);
                break;
            case "Sair":
                Messages.ShowEndProgram();
                return;
        }
    }
    
    public void Create()
    {
        var name = AskActivityName();

        var activity = new Activity { Name = name };
        context.Activities.Add(activity);
        context.SaveChanges();
        Messages.ShowSuccess($"Atividade [navy]{activity.Name}[/] criada com [green]sucesso[/].");
    }
    
    public void Update()
    {
        var id = AskActivityId();
        var activity = GetById(id);
        if (activity is null)
        {
            Messages.ShowError($"Atividade de código [navy]{id}[/] não foi encontrada.");
            return;
        }
        
        AnsiConsole.MarkupLine($"Id: {activity.Id}, Nome: {activity.Name}, Ativo: {activity.IsActive}");
        var confirmationName = AnsiConsole.Prompt(new ConfirmationPrompt("Deseja atualizar o [navy]nome[/]?"));
        if (confirmationName)
        {
            var newName = AskActivityName();
            activity.Name = newName;
        }
        
        var confirmationIsActive = AnsiConsole.Prompt(new ConfirmationPrompt("Deseja atualizar o [navy]status[/] da atividade?"));
        if (confirmationIsActive)
        {
            var newIsActive = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("Informe o novo [navy]status[/] da atividade:")
                .AddChoices("Ativo", "Inativo", "Cancelar")
            );
            switch (newIsActive)
            {
                case "Ativo":
                    activity.IsActive = true;
                    break;
                case "Inativo":
                    activity.IsActive = false;
                    break;
                case "Cancelar":
                    Messages.ShowEndProgram();
                    return;
            }
        }
        context.Entry(activity).State = EntityState.Modified;
        context.SaveChanges();
        Messages.ShowSuccess($"Atividade [navy]{activity.Name}[/] atualizada com [green]sucesso[/].");
    }
    
    public void Delete()
    {
        var id = AskActivityId();
        var activity = GetById(id);
        if (activity is null)
        {
            Messages.ShowError($"Atividade de código [navy]{id}[/] não foi encontrada.");
            return;
        }
        
        AnsiConsole.MarkupLine($"Id: {activity.Id}, Nome: {activity.Name}, Ativo: {activity.IsActive}");
        var confirmation = AnsiConsole.Prompt(new ConfirmationPrompt("Deseja [red]excluir permanentemente[/] essa atividade?"));
        if (!confirmation) return;
        
        context.Activities.Remove(activity);
        context.SaveChanges();
        Messages.ShowSuccess($"Atividade [navy]{activity.Name}[/] de código [navy]{activity.Id}[/] deletada com [green]sucesso[/].");
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
    
    public void ActivityTable(Activity activity)
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