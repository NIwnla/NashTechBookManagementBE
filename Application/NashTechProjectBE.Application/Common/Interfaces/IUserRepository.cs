using System.Linq.Expressions;
using NashTechProjectBE.Domain.Entities;

namespace NashTechProjectBE.Application.Common.Interfaces;

public interface IUserRepository
{
    IQueryable<User> GetAll();
    IQueryable<User> GetByCondition(Expression<Func<User, bool>> condition);
    Task<User> GetByIdAsync(Guid id);
    Task<User> GetByUserNameAsync(string userName);
    Task<User> GetByEmailAsync(string email);
    Task<bool> UserExist(Guid id);
    Task<bool> UpdateUseAsync(User user);
    Task<bool> DeleteUserAsync(Guid id);
    Task<bool> AddUserAsync(User user);
    Task<bool> SaveAsync();
}
