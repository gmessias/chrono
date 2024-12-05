using Chrono.Console.Database;
using Chrono.Console.Enums;
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
    
    public IEnumerable<Activity> GetLikeName(string name)
    {
        return context.Activities
            .AsNoTracking()
            .Where(x => EF.Functions.Like(x.Name.ToLower(), $"%{name.ToLower()}%"))
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
    
    public bool NameExists(string name)
    {
        return context.Activities.Any(x => x.Name.ToLower() == name.ToLower());
    }
    
    public void Create(Activity activity)
    {
        context.Activities.Add(activity);
        context.SaveChanges();
    }
    
    public void Update(Activity activity)
    {
        context.Entry(activity).State = EntityState.Modified;
        context.SaveChanges();
    }
    
    public void Delete(Activity activity)
    {
        context.Activities.Remove(activity);
        context.SaveChanges();
    }
}