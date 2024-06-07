using System.Linq.Expressions;
using NashTechProjectBE.Application.Common.Interfaces;
using NashTechProjectBE.Infractructure.Context;

namespace NashTechProjectBE.Infractructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly ApplicationDbContext _context;
    public GenericRepository(ApplicationDbContext context){
        _context = context;
    }
    public async Task<bool> CreateAsync(T entity, Guid? userId)
    {
        _context.Set<T>().Add(entity);
        return await SaveAsync(userId);
    }

    public async Task<bool> DeleteAsync(T entity)
    {
       _context.Set<T>().Remove(entity);
       return await SaveAsync();
       
    }
    public async Task<bool> DeleteRangeAsync(IEnumerable<T> entities){
        _context.Set<T>().RemoveRange(entities);
        return await SaveAsync();
    }

    public IQueryable<T> FindAll()
    {
        return _context.Set<T>();
    }

    public IQueryable<T> FindByCondition(Expression<Func<T, bool>> condition)
    {
        return _context.Set<T>().Where(condition);
    }

    public T1 GetShadowProperty<T1>(string propertyName, T entity)
    {
        return _context.GetShadowProperty<T1>(propertyName, entity);
    }

    public async Task<bool> SaveAsync(Guid? userId = null)
    {
        return await _context.SaveChangesAsync(userId) > 0 ? true : false;
    }

    public async Task<bool> UpdateAsync(T entity, Guid? userId)
    {
        _context.Set<T>().Update(entity);
        return await SaveAsync(userId);
    }

    public async Task<bool> UpdateRangeAsync(IEnumerable<T> entities, Guid? userId)
    {
        _context.Set<T>().UpdateRange(entities);
        return await SaveAsync(userId);
    }
}
