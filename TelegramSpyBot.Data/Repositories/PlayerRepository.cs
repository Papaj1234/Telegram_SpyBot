namespace TelegramSpyBot.Data.Repositories;

public class PlayerRepository : Repository<Player>, IPlayerRepository
{
    public PlayerRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Player>> GetBySessionAsync(int sessionId)
        => await DbSet
            .Where(p => p.SessionId == sessionId)
            .ToListAsync();
}