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
        return context.Activities.AsNoTracking().ToList();
    }
    
    public Activity? GetById(long id)
    {
        return context.Activities.FirstOrDefault(x => x.Id == id);
    }
    
    public void Create()
    {
        var name = AnsiConsole.Prompt(new TextPrompt<string>("Informe o [navy]nome[/] da atividade:")
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

        var activity = new Activity { Name = name };
        context.Activities.Add(activity);
        context.SaveChanges();
        AnsiConsole.MarkupLine($"Atividade [navy]{activity.Name}[/] criada com [green]sucesso[/].");
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