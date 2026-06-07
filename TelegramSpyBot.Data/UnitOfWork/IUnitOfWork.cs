using TelegramSpyBot.Data.Repositories;

namespace TelegramSpyBot.Data.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    ICharacterRepository Characters { get; }

    IUniverseRepository Universes { get; }

    IGameSessionRepository GameSessions { get; }

    IPlayerRepository Players { get; }

    Task<int> SaveChangesAsync();
}