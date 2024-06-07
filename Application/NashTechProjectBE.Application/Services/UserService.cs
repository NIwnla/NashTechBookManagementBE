using System.Security.Cryptography;
using System.Text;
using NashTechProjectBE.Application.Common.Interfaces;
using NashTechProjectBE.Application.Common.ViewModel;
using NashTechProjectBE.Domain.Entities;
using NashTechProjectBE.Domain.Contraints;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using NashTechProjectBE.Application.Common.Models;
using System.Net;

namespace NashTechProjectBE.Application.Services;

public class UserService : Roles,IUserService
{
    private readonly IUserRepository _repository;
    public UserService(IUserRepository repository)
    {
        _repository = repository;
    }
    public bool CheckPassword(string password, User user)
    {
        bool result;
        using (HMACSHA512? hmac = new HMACSHA512(user.PasswordSalt))
        {
            var compute = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            result = compute.SequenceEqual(user.PasswordHash);
        }
        return result;
    }

    public async Task<User> GetByUsernameAsync(string username)
    {
        return await _repository.GetByUserNameAsync(username);
    }

    public async Task<Result> LoginAsync(LoginViewModel login, byte[] key)
    {
        var user = await _repository.GetByUserNameAsync(login.Username);
        if (user == null)
        {
            return new Result { StatusCode = HttpStatusCode.BadRequest, Message = "UserName or password incorrect" };
        }
        var match = CheckPassword(login.Password, user);
        if (!match)
        {
            return new Result { StatusCode = HttpStatusCode.BadRequest, Message = "UserName or password incorrect" };
        }
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("username", user.Username), new Claim(ClaimTypes.Role, user.Role.ToUpper()), new Claim("id",user.Id.ToString()), new Claim("requestCount", user.RequestCount.ToString()) }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var encrypterToken = tokenHandler.WriteToken(token);
        return new Result { StatusCode = HttpStatusCode.OK, Message = encrypterToken };
    }

    public async Task<Result> RegisterAsync(RegisterViewModel register)
    {
        var user = new User { Username = register.Username, Role = register.Role };
        user.RequestCount = 3;
        user.CountResetDate = DateTime.UtcNow.AddMonths(1);
        var userExisted = await _repository.GetByUserNameAsync(user.Username);
        if (userExisted != null)
        {
            return new Result { StatusCode = HttpStatusCode.BadRequest, Message = "UserName already exist" };
        }

        using (HMACSHA512? hmac = new HMACSHA512())
        {
            user.PasswordSalt = hmac.Key;
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(register.Password));
        }

        if (await _repository.AddUserAsync(user))
        {
            return new Result { StatusCode = HttpStatusCode.OK, Message = "User successfully registered" };
        }
        else
        {
            return new Result { StatusCode = HttpStatusCode.InternalServerError, Message = "Error when registering user" };
        }
    }

}
