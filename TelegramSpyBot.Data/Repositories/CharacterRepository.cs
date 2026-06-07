namespace TelegramSpyBot.Data.Repositories;

public class CharacterRepository : Repository<Character>, ICharacterRepository
{
    public CharacterRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Character>> GetByUniverseAsync(int universeId)
        => await DbSet
            .Where(c => c.UniverseId == universeId)
            .ToListAsync();

    public async Task<Character?> GetRandomByUniverseAsync(int universeId)
        => await DbSet
            .Where(c => c.UniverseId == universeId)
            .OrderBy(_ => EF.Functions.Random())
            .FirstOrDefaultAsync();
}