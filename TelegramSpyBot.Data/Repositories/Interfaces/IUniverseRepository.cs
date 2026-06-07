namespace TelegramSpyBot.Data.Repositories.Interfaces;

public interface IUniverseRepository : IRepository<Universe>
{
    Task<Universe?> GetByNameAsync(string name);
}