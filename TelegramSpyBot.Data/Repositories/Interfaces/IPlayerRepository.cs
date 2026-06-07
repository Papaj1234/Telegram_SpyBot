namespace TelegramSpyBot.Data.Repositories.Interfaces;

public interface IPlayerRepository : IRepository<Player>
{
    Task<IReadOnlyList<Player>> GetBySessionAsync(int sessionId);
}