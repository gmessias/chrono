using Chrono.Console.Models;

namespace Chrono.Console.Repositories.Interfaces;

public interface ITimeRepository
{
    IEnumerable<Time> GetAll();
    Time? GetById(long id);
    IEnumerable<Time> GetAllTimeByActivity(string activityName);
    IEnumerable<Time> GetAllTimeByDay(string date);
    IEnumerable<Time> GetAllTimeByMonth(string monthYear);
    void Create(Time time);
    void Update(Time time);
    void Delete(Time time);
}