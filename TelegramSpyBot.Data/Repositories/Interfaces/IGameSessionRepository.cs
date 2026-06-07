namespace TelegramSpyBot.Data.Repositories.Interfaces;

public interface IGameSessionRepository : IRepository<GameSession>
{
    Task<GameSession?> GetActiveByChatAsync(long chatId);

    Task<GameSession?> GetWithPlayersAsync(int sessionId);
}