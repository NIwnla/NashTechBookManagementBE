using NashTechProjectBE.Application.Common.Models;
using NashTechProjectBE.Application.Common.ViewModel;
using NashTechProjectBE.Domain.Entities;

namespace NashTechProjectBE.Application.Common.Interfaces;

public interface IUserService
{
   Task<Result> LoginAsync(LoginViewModel model, byte[] key);
   Task<Result> RegisterAsync(RegisterViewModel model);
   bool CheckPassword(string password, User user);

   Task<User> GetByUsernameAsync(string username);
}
