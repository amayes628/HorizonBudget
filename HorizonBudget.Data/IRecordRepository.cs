namespace HorizonBudget.Data;

/// <summary>
/// A dedicated repository structure for modern, immutable record-backed entities.
/// </summary>
public interface IRecordRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(Guid id);
}
