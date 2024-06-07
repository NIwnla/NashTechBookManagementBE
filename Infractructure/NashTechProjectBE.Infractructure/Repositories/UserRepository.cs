using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NashTechProjectBE.Application.Common.Interfaces;
using NashTechProjectBE.Domain.Entities;
using NashTechProjectBE.Infractructure.Context;

namespace NashTechProjectBE.Infractructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;
    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> AddUserAsync(User user)
    {
        var userExist = await GetByUserNameAsync(user.Username);
        if(userExist == null){
            await _context.AddAsync(user);
        }
        return await SaveAsync();
    }

    public async Task<bool> DeleteUserAsync(Guid id)
    {
        var user = await GetByIdAsync(id);
        if (user != null){
            _context.Users.Remove(user);
        }
        return await SaveAsync();
    }


    public IQueryable<User> GetAll()
    {
        return _context.Users.AsQueryable();
    }

    public IQueryable<User> GetByCondition(Expression<Func<User, bool>> condition)
    {
        return _context.Users.Where(condition);
    }


    public async Task<User> GetByEmailAsync(string email)
    {
        return await _context.Users.Where(u => u.Email == email).FirstOrDefaultAsync();
    }

    public async Task<User> GetByIdAsync(Guid id)
    {
        return await _context.Users.Where(u => u.Id == id).FirstOrDefaultAsync();
    }

    public async Task<User> GetByUserNameAsync(string userName)
    {
        return await _context.Users.Where(u => u.Username == userName).FirstOrDefaultAsync();
    }


    public async Task<bool> SaveAsync()
    {
        return await _context.SaveChangesAsync() > 0 ? true : false;
    }

    public async Task<bool> UpdateUseAsync(User user)
    {
        _context.Update(user);
        return await SaveAsync();
    }

    public async Task<bool> UserExist(Guid id)
    {
       return await _context.Users.AnyAsync(u => u.Id == id);
    }
}
