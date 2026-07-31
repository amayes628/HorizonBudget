using Microsoft.EntityFrameworkCore;

namespace HorizonBudget.Data;

public class RecordRepository<T>(HorizonBudgetContext context) : IRecordRepository<T>
where T : class
{
    private readonly HorizonBudgetContext _context = context;

    public async Task AddAsync(T entity)
    {
        _context.Set<T>().Add(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _context.Set<T>().FindAsync(id);
        if (entity != null)
        {
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IReadOnlyList<T>> GetAllAsync() => await _context.Set<T>().ToListAsync();

    public async Task<T?> GetByIdAsync(Guid id) => await _context.Set<T>().FindAsync(id);


    public async Task UpdateAsync(T entity)
    {
        _context.Set<T>().Update(entity);
        await _context.SaveChangesAsync();
    }
}
