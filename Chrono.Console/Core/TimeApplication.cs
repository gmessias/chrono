using Chrono.Console.Enums;
using Chrono.Console.Models;
using Chrono.Console.Repositories.Interfaces;
using Spectre.Console;

namespace Chrono.Console.Core;

public sealed class TimeApplication(ITimeRepository timeRepository, IActivityRepository activityRepository)
{
    public void Views()
    {
        var select = AnsiConsole.Prompt(new SelectionPrompt<EnumTimeViewsOptions>()
            .Title("Escolha uma opção de visualização:")
            .AddChoices(Enum.GetValues<EnumTimeViewsOptions>())
            .UseConverter(option => option.GetDescription())
        );
        
        var activityApplication = new ActivityApplication(activityRepository);
        
        switch (select)
        {
            case EnumTimeViewsOptions.ListarTodos:
                TimeTable(timeRepository.GetAll());
                break;
            case EnumTimeViewsOptions.BuscarPorCodigo:
                var time = timeRepository.GetById(AskTimeId());
                if (time is null)
                {
                    Messages.ShowError("Marcação [maroon]não[/] foi encontrada.");
                    return;
                }
                TimeTable(time);
                break;
            case EnumTimeViewsOptions.BuscarPorAtividade:
                var nameActivity = activityApplication.AskActivityName();
                var activity = activityRepository.GetByName(nameActivity);
                if (activity is null)
                {
                    Messages.ShowWarning("Atividade [maroon]não[/] encontrada, verifique abaixo as [navy]atividades ativas[/] e tente novamente.");
                    activityApplication.ActivityTable(activityRepository.GetAllActive());
                    var idActivity = activityApplication.AskActivityId();
                    activity = activityRepository.GetById(idActivity);
                }
                if (activity is null)
                {
                    Messages.ShowError("Atividade [maroon]não[/] foi encontrada, finalizando o programa.");
                    return;
                }

                TimeTable(timeRepository.GetAllTimeByActivity(activity.Name));
                break;
            case EnumTimeViewsOptions.BuscarPorDia:
                var date = AskTimeDate();
                TimeTable(timeRepository.GetAllTimeByDay(date));
                break;
            case EnumTimeViewsOptions.BuscarPorMes:
                var monthYear = AskMonthYear();
                TimeTable(timeRepository.GetAllTimeByMonth(monthYear));
                break;
            case EnumTimeViewsOptions.Sair:
                Messages.ShowEndProgram();
                return;
        }
    }
    
    public void Create()
    {
        var description = AskTimeDescription();
        
        var activityApplication = new ActivityApplication(activityRepository);
        var nameActivity = activityApplication.AskActivityName();
        
        var activity = activityRepository.GetByName(nameActivity);
        if (activity is null)
        {
            Messages.ShowWarning("Atividade [maroon]não[/] encontrada, verifique abaixo as [navy]atividades ativas[/] e tente novamente.");
            activityApplication.ActivityTable(activityRepository.GetAllActive());
            var idActivity = activityApplication.AskActivityId();
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
        
        try
        {
            timeRepository.Create(time);
            Messages.ShowSuccess($"Marcação [navy]{time.Name}[/] criada com [green]sucesso[/].");
        }
        catch (Exception ex)
        {
            Messages.ShowError("Marcação [maroon]não[/] foi criado.");
            System.Console.WriteLine(ex.Message);
        }
    }
    
    public void Update()
    {
        var id = AskTimeId();
        var time = timeRepository.GetById(id);
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
            var activityApplication = new ActivityApplication(activityRepository);
            var nameActivity = activityApplication.AskActivityName();
            var newActivity = activityRepository.GetByName(nameActivity);
            if (newActivity is null)
            {
                Messages.ShowWarning("Atividade [maroon]não[/] encontrada, verifique abaixo as [navy]atividades ativas[/] e tente novamente.");
                activityApplication.ActivityTable(activityRepository.GetAllActive());
                var idActivity = activityApplication.AskActivityId();
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
        
        try
        {
            timeRepository.Update(time);
            Messages.ShowSuccess($"A marcação de código [navy]{time.Id}[/] foi atualizada com [green]sucesso[/].");
        }
        catch (Exception ex)
        {
            Messages.ShowError("Marcação [maroon]não[/] foi atualizada.");
            System.Console.WriteLine(ex.Message);
        }
    }
    
    public void Delete()
    {
        var id = AskTimeId();
        var time = timeRepository.GetById(id);
        if (time is null)
        {
            Messages.ShowError($"Marcação de código [navy]{id}[/] não foi encontrada.");
            return;
        }
        
        AnsiConsole.MarkupLine($"Id: {time.Id}, Descrição: {time.Name}, Atividade: {time.Activity.Name}, Tempo gasto: {time.TimeCount}, Data: {time.Date}");
        var confirmation = AnsiConsole.Prompt(new ConfirmationPrompt("Deseja [red]excluir permanentemente[/] essa marcação?"));
        if (!confirmation) return;
        
        try
        {
            timeRepository.Delete(time);
            Messages.ShowSuccess($"Marcação de código [navy]{time.Id}[/] deletada com [green]sucesso[/].");
        }
        catch (Exception ex)
        {
            Messages.ShowError("Marcação [maroon]não[/] foi excluída.");
            System.Console.WriteLine(ex.Message);
        }
    }
    
    private long AskTimeId()
    {
        return AnsiConsole.Prompt(new TextPrompt<long>("Informe o [navy]código[/] da marcação:")
            .Validate(input => input > 0 
                ? ValidationResult.Success() 
                : ValidationResult.Error("[red]Código deve ser maior que zero.[/]"))
        );
    }
    
    private string AskTimeDescription()
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
    
    private string AskTimeTimeCount()
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
    
    private string AskTimeDate()
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
    
    private string AskMonthYear()
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