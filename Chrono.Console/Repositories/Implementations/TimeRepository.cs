using Chrono.Console.Database;
using Chrono.Console.Models;
using Chrono.Console.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Chrono.Console.Repositories.Implementations;

public sealed class TimeRepository(ApplicationDbContext context) : ITimeRepository
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
    
    public void Create(Time time)
    {
        context.Times.Add(time);
        context.SaveChanges();
    }

    public void Update(Time time)
    {
        context.Entry(time).State = EntityState.Modified;
        context.SaveChanges();
    }

    public void Delete(Time time)
    {
        context.Times.Remove(time);
        context.SaveChanges();
    }
}