namespace Fundo.Loan.Application.Abstractions;
public interface IRepository<T> where T : class
{
    ValueTask<T> GetByIdAsync(object key);
    void Add(T entity);
    void Update(T entity);
    void Delete(T entity);
}
