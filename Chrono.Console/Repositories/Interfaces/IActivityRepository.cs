using Chrono.Console.Models;

namespace Chrono.Console.Repositories.Interfaces;

public interface IActivityRepository
{
    IEnumerable<Activity> GetAll();
    IEnumerable<Activity> GetAllActive();
    Activity? GetById(long id);
    Activity? GetByName(string name);
    IEnumerable<Activity> GetLikeName(string name);
    void Views();
    void Create();
    void Update();
    void Delete();
    long AskActivityId();
    string AskActivityName();
    void ActivityTable(Activity activity);
    void ActivityTable(IEnumerable<Activity> activities);
}