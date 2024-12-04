using System.ComponentModel.DataAnnotations;

namespace Chrono.Console.Models;

public sealed class Time
{
    public long Id { get; init; }
    [MaxLength(32)]
    public string Name { get; set; } = string.Empty;
    public required Activity Activity { get; set; }
    public string TimeCount 
    {
        get => _timeCount.ToString(@"hh\:mm"); 
        set
        {
            if (TimeSpan.TryParseExact(
                value, 
                @"hh\:mm", 
                null, 
                out var parsedTime))
            {
                _timeCount = parsedTime;
            } else {
                throw new FormatException("O formato do tempo gasto deve ser hh:mm.");
            }
        }
    }
    public string Date 
    {
        get => _date.ToString("dd/MM/yyyy"); 
        set
        {
            if (DateTime.TryParseExact(
                value, 
                "dd/MM/yyyy", 
                null, 
                System.Globalization.DateTimeStyles.None, 
                out var parsedDate))
            {
                _date = parsedDate;
            } else {
                throw new FormatException("O formato da data de marcação deve ser dd/MM/yyyy.");
            }
        }
    }
    private TimeSpan _timeCount;
    private DateTime _date;
}