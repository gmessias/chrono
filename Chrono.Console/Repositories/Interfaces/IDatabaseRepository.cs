namespace Chrono.Console.Repositories.Interfaces;

public interface IDatabaseRepository
{
    bool DatabaseExists();
    void ApplyMigrations();
    bool Delete();
}