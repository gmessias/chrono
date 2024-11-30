using Chrono.Console.Models;

namespace Chrono.Console.Repositories.Interfaces;

public interface IActivityRepository
{
    IEnumerable<Activity> GetAll();
    Activity? GetById(long id);
    void Create();
    void ActivityTable(Activity activity);
    void ActivityTable(IEnumerable<Activity> activities);
}