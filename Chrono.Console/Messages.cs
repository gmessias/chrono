using Spectre.Console;

namespace Chrono.Console;

public class Messages
{
    public static void ShowSuccess(string message)
    {
        AnsiConsole.MarkupLine($"[green]Success[/]: {message}");
    }
    
    public static void ShowError(string message)
    {
        AnsiConsole.MarkupLine($"[maroon]Error[/]: {message}");
    }
    
    public static void ShowWarning(string message)
    {
        AnsiConsole.MarkupLine($"[olive]Warning[/]: {message}");
    }
    
    public static void ShowRule(string message, Justify position)
    {
        var rule = new Rule($"[navy]{message}[/]")
        {
            Justification = position
        };
        AnsiConsole.Write(rule);
    }
}