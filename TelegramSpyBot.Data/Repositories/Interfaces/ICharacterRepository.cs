namespace TelegramSpyBot.Data.Repositories.Interfaces;

public interface ICharacterRepository : IRepository<Character>
{
    Task<IReadOnlyList<Character>> GetByUniverseAsync(int universeId);

    Task<Character?> GetRandomByUniverseAsync(int universeId);
}