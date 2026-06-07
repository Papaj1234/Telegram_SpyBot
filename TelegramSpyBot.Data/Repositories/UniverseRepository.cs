namespace TelegramSpyBot.Data.Repositories;

public class UniverseRepository : Repository<Universe>, IUniverseRepository
{
    public UniverseRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Universe?> GetByNameAsync(string name)
        => await DbSet
            .FirstOrDefaultAsync(u => u.Name == name);
}