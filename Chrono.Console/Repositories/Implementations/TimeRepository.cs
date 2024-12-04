using Chrono.Console.Database;
using Chrono.Console.Enums;
using Chrono.Console.Models;
using Chrono.Console.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;

namespace Chrono.Console.Repositories.Implementations;

public sealed class TimeRepository(ApplicationDbContext context, IActivityRepository activityRepository) : ITimeRepository
{
    public IEnumerable<Time> GetAll()
    {
        return context.Times
            .Include(x => x.Activity)
            .AsNoTracking()
            .ToList();
    }
    
    public Time? GetById(long id)
    {
        return context.Times
            .Include(x => x.Activity)
            .FirstOrDefault(x => x.Id == id);
    }
    
    public IEnumerable<Time> GetAllTimeByActivity(string activityName)
    {
        return context.Times
            .Include(x => x.Activity)
            .AsNoTracking()
            .Where(x => x.Activity.Name == activityName)
            .ToList();
    }
    
    public IEnumerable<Time> GetAllTimeByDay(string date)
    {
        return context.Times
            .Include(x => x.Activity)
            .AsNoTracking()
            .Where(x => x.Date == date)
            .ToList();
    }
    
    public IEnumerable<Time> GetAllTimeByMonth(string monthYear)
    {
        if (!DateTime.TryParseExact(monthYear, "MM/yyyy", null, System.Globalization.DateTimeStyles.None, out var parsedMonthYear))
        {
            throw new FormatException("O formato deve ser MM/yyyy.");
        }
        
        var month = parsedMonthYear.Month;
        var year = parsedMonthYear.Year;

        return context.Times
            .Include(x => x.Activity)
            .AsEnumerable()
            .Where(t => DateTime.TryParseExact(t.Date, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out var date)
                        && date.Month == month 
                        && date.Year == year)
            .ToList();
    }
    
    public void Views()
    {
        Messages.ShowRule("Visualização de marcações");
        var select = AnsiConsole.Prompt(new SelectionPrompt<EnumTimeViewsOptions>()
            .Title("Escolha uma opção de visualização:")
            .AddChoices(Enum.GetValues<EnumTimeViewsOptions>())
            .UseConverter(option => option.GetDescription())
        );
        
        switch (select)
        {
            case EnumTimeViewsOptions.ListarTodos:
                TimeTable(GetAll());
                break;
            case EnumTimeViewsOptions.BuscarPorCodigo:
                var time = GetById(AskTimeId());
                if (time is null)
                {
                    Messages.ShowError("Marcação [maroon]não[/] foi encontrada.");
                    return;
                }
                TimeTable(time);
                break;
            case EnumTimeViewsOptions.BuscarPorAtividade:
                var nameActivity = activityRepository.AskActivityName();
                var activity = activityRepository.GetByName(nameActivity);
                if (activity is null)
                {
                    Messages.ShowWarning("Atividade [maroon]não[/] encontrada, verifique abaixo as [navy]atividades ativas[/] e tente novamente.");
                    activityRepository.ActivityTable(activityRepository.GetAllActive());
                    var idActivity = activityRepository.AskActivityId();
                    activity = activityRepository.GetById(idActivity);
                }
                if (activity is null)
                {
                    Messages.ShowError("Atividade [maroon]não[/] foi encontrada, finalizando o programa.");
                    return;
                }

                TimeTable(GetAllTimeByActivity(activity.Name));
                break;
            case EnumTimeViewsOptions.BuscarPorDia:
                var date = AskTimeDate();
                TimeTable(GetAllTimeByDay(date));
                break;
            case EnumTimeViewsOptions.BuscarPorMes:
                var monthYear = AskMonthYear();
                TimeTable(GetAllTimeByMonth(monthYear));
                break;
            case EnumTimeViewsOptions.Sair:
                Messages.ShowEndProgram();
                return;
        }
    }

    public void Create()
    {
        var description = AskTimeDescription();
        var nameActivity = activityRepository.AskActivityName();
        var activity = activityRepository.GetByName(nameActivity);
        if (activity is null)
        {
            Messages.ShowWarning("Atividade [maroon]não[/] encontrada, verifique abaixo as [navy]atividades ativas[/] e tente novamente.");
            activityRepository.ActivityTable(activityRepository.GetAllActive());
            var idActivity = activityRepository.AskActivityId();
            activity = activityRepository.GetById(idActivity);
        }
        if (activity is null)
        {
            Messages.ShowError("Atividade [maroon]não[/] foi encontrada, finalizando o programa.");
            return;
        }

        var timeCount = AskTimeTimeCount();
        var date = AskTimeDate();
        
        var time = new Time 
        {
            Name = description,
            Activity = activity,
            TimeCount = timeCount,
            Date = date
        };
        context.Times.Add(time);
        context.SaveChanges();
        Messages.ShowSuccess($"Marcação [navy]{time.Name}[/] criada com [green]sucesso[/].");
    }

    public void Update()
    {
        var id = AskTimeId();
        var time = GetById(id);
        if (time is null)
        {
            Messages.ShowError($"Marcação de código [navy]{id}[/] não foi encontrada.");
            return;
        }
        
        AnsiConsole.MarkupLine($"Id: {time.Id}, Descrição: {time.Name}, Atividade: {time.Activity.Name}, Tempo gasto: {time.TimeCount}, Data: {time.Date}");
        var confirmationDescription = AnsiConsole.Prompt(new ConfirmationPrompt("Deseja atualizar a [navy]descrição[/]?"));
        if (confirmationDescription)
        {
            var newDescription = AskTimeDescription();
            time.Name = newDescription;
        }
        
        var confirmationActivity = AnsiConsole.Prompt(new ConfirmationPrompt("Deseja atualizar a [navy]atividade[/]?"));
        if (confirmationActivity)
        {
            var nameActivity = activityRepository.AskActivityName();
            var newActivity = activityRepository.GetByName(nameActivity);
            if (newActivity is null)
            {
                Messages.ShowWarning("Atividade [maroon]não[/] encontrada, verifique abaixo as [navy]atividades ativas[/] e tente novamente.");
                activityRepository.ActivityTable(activityRepository.GetAllActive());
                var idActivity = activityRepository.AskActivityId();
                newActivity = activityRepository.GetById(idActivity);
            }
            if (newActivity is null)
            {
                Messages.ShowError("Atividade [maroon]não[/] foi encontrada, finalizando o programa.");
                return;
            }
            time.Activity = newActivity;
        }
        
        var confirmationTimeCount = AnsiConsole.Prompt(new ConfirmationPrompt("Deseja atualizar o [navy]tempo gasto[/]?"));
        if (confirmationTimeCount)
        {
            var newTimeCount = AskTimeTimeCount();
            time.TimeCount = newTimeCount;
        }
        
        var confirmationDate = AnsiConsole.Prompt(new ConfirmationPrompt("Deseja atualizar a [navy]data[/]?"));
        if (confirmationDate)
        {
            var newDate = AskTimeDate();
            time.Date = newDate;
        }
        
        context.Entry(time).State = EntityState.Modified;
        context.SaveChanges();
        Messages.ShowSuccess($"A marcação de código [navy]{time.Id}[/] foi atualizada com [green]sucesso[/].");
    }

    public void Delete()
    {
        var id = AskTimeId();
        var time = GetById(id);
        if (time is null)
        {
            Messages.ShowError($"Marcação de código [navy]{id}[/] não foi encontrada.");
            return;
        }
        
        AnsiConsole.MarkupLine($"Id: {time.Id}, Descrição: {time.Name}, Atividade: {time.Activity.Name}, Tempo gasto: {time.TimeCount}, Data: {time.Date}");
        var confirmation = AnsiConsole.Prompt(new ConfirmationPrompt("Deseja [red]excluir permanentemente[/] essa marcação?"));
        if (!confirmation) return;
        
        context.Times.Remove(time);
        context.SaveChanges();
        Messages.ShowSuccess($"Marcação de código [navy]{time.Id}[/] deletada com [green]sucesso[/].");
    }
    
    private static long AskTimeId()
    {
        return AnsiConsole.Prompt(new TextPrompt<long>("Informe o [navy]código[/] da marcação:")
            .Validate(input => input > 0 
                ? ValidationResult.Success() 
                : ValidationResult.Error("[red]Código deve ser maior que zero.[/]"))
        );
    }
    
    private static string AskTimeDescription()
    {
        return AnsiConsole.Prompt(new TextPrompt<string>("Informe a [navy]descrição[/] da marcação:")
            .Validate(input => 
            {
                if (input.Length > 32)
                {
                    return ValidationResult.Error("[red]A descrição da marcação não pode ter mais de 32 caracteres.[/]");
                }
                return ValidationResult.Success();
            })
        );
    }
    
    private static string AskTimeTimeCount()
    {
        return AnsiConsole.Prompt(new TextPrompt<string>("Informe o [navy]tempo gasto[/] no formato [olive]hh:mm[/]:")
            .Validate(input => 
            {
                if (string.IsNullOrWhiteSpace(input))
                {
                    return ValidationResult.Error("[red]O tempo gasto deve ser preenchido.[/]");
                }
                return ValidationResult.Success();
            })
        );
    }
    
    private static string AskTimeDate()
    {
        return AnsiConsole.Prompt(new TextPrompt<string>("Informe a [navy]data[/] no formato [olive]dd/MM/yyyy[/]:")
            .Validate(input => 
            {
                if (string.IsNullOrWhiteSpace(input))
                {
                    return ValidationResult.Error("[red]A data deve ser preenchido.[/]");
                }
                return ValidationResult.Success();
            })
        );
    }
    
    private static string AskMonthYear()
    {
        return AnsiConsole.Prompt(new TextPrompt<string>("Informe o [navy]mês e ano[/] no formato [olive]MM/yyyy[/]:")
            .Validate(input =>
            {
                if (string.IsNullOrWhiteSpace(input))
                {
                    return ValidationResult.Error("[red]O mês e ano devem ser preenchidos.[/]");
                }

                if (!DateTime.TryParseExact(input, "MM/yyyy", null, System.Globalization.DateTimeStyles.None, out _))
                {
                    return ValidationResult.Error("[red]Formato inválido. Use MM/yyyy.[/]");
                }

                return ValidationResult.Success();
            })
        );
    }
    
    private static void TimeTable(Time time)
    {
        var times = new List<Time> { time };
        TimeTable(times);
    }
    
    private static void TimeTable(IEnumerable<Time> times)
    {
        var table = new Table();
        table.AddColumn(new TableColumn("Id").Centered());
        table.AddColumn(new TableColumn("Descrição").Centered());
        table.AddColumn(new TableColumn("Atividade").Centered());
        table.AddColumn(new TableColumn("Tempo gasto").Centered());
        table.AddColumn(new TableColumn("Data (dd/MM/yyyy)").Centered());
        
        var totalTime = TimeSpan.Zero; 
        foreach (var time in times)
        {
            table.AddRow(
                time.Id.ToString(), 
                time.Name, 
                time.Activity.Name, 
                time.TimeCount, 
                time.Date);
            
            if (TimeSpan.TryParseExact(time.TimeCount, @"hh\:mm", null, out var parsedTime))
            {
                totalTime += parsedTime;
            }
        }
        AnsiConsole.Write(table);
        Messages.ShowSuccess($"Total de [navy]tempo gasto[/]: {totalTime:hh\\:mm}");
    }
}