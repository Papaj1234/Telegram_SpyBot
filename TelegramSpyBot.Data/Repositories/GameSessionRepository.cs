namespace TelegramSpyBot.Data.Repositories;

public class GameSessionRepository : Repository<GameSession>, IGameSessionRepository
{
    public GameSessionRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<GameSession?> GetActiveByChatAsync(long chatId)
        => await DbSet
            .Include(s => s.Players)
            .Include(s => s.Character)
            .FirstOrDefaultAsync(s =>
                s.ChatId == chatId &&
                s.Status != GameStatus.Finished);

    public async Task<GameSession?> GetWithPlayersAsync(int sessionId)
        => await DbSet
            .Include(s => s.Players)
            .Include(s => s.Character)
            .FirstOrDefaultAsync(s => s.Id == sessionId);
}