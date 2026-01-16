using System.Linq.Expressions;
using SoftwareQualityAssurance.Models;

namespace SoftwareQualityAssurance.Repositories;

public interface IRepository<T> where T : BaseEntity
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T> AddAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task<bool> DeleteAsync(int id); // Soft delete
    Task<bool> ExistsAsync(int id);
}

