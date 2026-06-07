using TelegramSpyBot.Data.Repositories;

namespace TelegramSpyBot.Data.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext context;

    public UnitOfWork(AppDbContext context)
    {
        this.context = context;
        Characters = new CharacterRepository(context);
        Universes = new UniverseRepository(context);
        GameSessions = new GameSessionRepository(context);
        Players = new PlayerRepository(context);
    }

    public ICharacterRepository Characters { get; }
    public IUniverseRepository Universes { get; }
    public IGameSessionRepository GameSessions { get; }
    public IPlayerRepository Players { get; }

    public async Task<int> SaveChangesAsync()
        => await this.context.SaveChangesAsync();

    public void Dispose()
        => this.context.Dispose();
}