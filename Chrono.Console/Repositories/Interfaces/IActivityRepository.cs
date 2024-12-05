using Chrono.Console.Models;

namespace Chrono.Console.Repositories.Interfaces;

public interface IActivityRepository
{
    IEnumerable<Activity> GetAll();
    IEnumerable<Activity> GetAllActive();
    IEnumerable<Activity> GetLikeName(string name);
    Activity? GetById(long id);
    Activity? GetByName(string name);
    bool NameExists(string name);
    void Create(Activity activity);
    void Update(Activity activity);
    void Delete(Activity activity);
}