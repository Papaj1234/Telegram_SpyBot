namespace TelegramSpyBot.Data.Repositories.Interfaces;

public interface IRepository<T>
    where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id);

    Task<IReadOnlyList<T>> GetAllAsync();

    Task AddAsync(T entity);

    void Update(T entity);

    void Remove(T entity);
}