using Fundo.Loan.Application.Abstractions;

namespace Fundo.Loan.Infrastructure.Data.Common;
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ApplicationDbContext _context;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
    }

    public void Add(T entity)
    {
        _context.Set<T>().Add(entity);
    }

    public void Delete(T entity)
    {
        _context.Set<T>().Remove(entity);
    }

    public async ValueTask<T> GetByIdAsync(object key)
    {
        return await _context.Set<T>().FindAsync(key);
    }

    public void Update(T entity)
    {
        _context.Set<T>().Update(entity);
    }
}
