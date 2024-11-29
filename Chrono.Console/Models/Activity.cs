namespace Chrono.Console.Models;

public sealed class Activity
{
    public long Id { get; init ; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}