using System.Linq.Expressions;

namespace NashTechProjectBE.Application.Common.Interfaces;

public interface IGenericRepository<T> where T : class
{
    IQueryable<T> FindAll();
    IQueryable<T> FindByCondition(Expression<Func<T, bool>> condition);
    Task<bool> CreateAsync(T entity, Guid? userId);
    Task<bool> UpdateAsync(T entity, Guid? userId);
    Task<bool> UpdateRangeAsync(IEnumerable<T> entities, Guid? userId);
    Task<bool> DeleteAsync(T entity);
    Task<bool> DeleteRangeAsync(IEnumerable<T> entities);
    T1 GetShadowProperty<T1>(string propertyName, T entity);
    Task<bool> SaveAsync(Guid? userId);
}
