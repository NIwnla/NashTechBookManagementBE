using FluentAssertions;
using Moq;
using NashTechProjectBE.Application.Common.Interfaces;
using NashTechProjectBE.Application.Common.ViewModel;
using NashTechProjectBE.Application.Services;
using NashTechProjectBE.Domain.Entities;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NashTechProjectBE.Test.Controllers
{
	[TestFixture]
	public class UserServiceTest
	{
		private Mock<IUserRepository> _repositoryMock;
		private UserService _userService;

		[SetUp]
		public void SetUp()
		{
			_repositoryMock = new Mock<IUserRepository>();
			_userService = new UserService(_repositoryMock.Object);
		}

		[Test]
		public async Task GetByUsernameAsync_ShouldReturnUser_WhenUserExists()
		{
			// Arrange
			var username = "testuser";
			var user = new User { Username = username };
			_repositoryMock.Setup(repo => repo.GetByUserNameAsync(username))
				.ReturnsAsync(user);

			// Act
			var result = await _userService.GetByUsernameAsync(username);

			// Assert
			result.Should().Be(user);
		}

		[Test]
		public async Task LoginAsync_ShouldReturnBadRequest_WhenUserDoesNotExist()
		{
			// Arrange
			var login = new LoginViewModel { Username = "testuser", Password = "password" };
			byte[] key = Encoding.ASCII.GetBytes("supersecretkey");
			_repositoryMock.Setup(repo => repo.GetByUserNameAsync(login.Username))
				.ReturnsAsync((User?)null);

			// Act
			var result = await _userService.LoginAsync(login, key);

			// Assert
			result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
			result.Message.Should().Be("UserName or password incorrect");
		}

		[Test]
		public async Task RegisterAsync_ShouldReturnOk_WhenUserSuccessfullyRegistered()
		{
			// Arrange
			var register = new RegisterViewModel { Username = "newuser", Password = "password", Role = "User" };
			_repositoryMock.Setup(repo => repo.GetByUserNameAsync(register.Username))
				.ReturnsAsync((User?)null);
			_repositoryMock.Setup(repo => repo.AddUserAsync(It.IsAny<User>()))
				.ReturnsAsync(true);

			// Act
			var result = await _userService.RegisterAsync(register);

			// Assert
			result.StatusCode.Should().Be(HttpStatusCode.OK);
			result.Message.Should().Be("User successfully registered");
		}

		[Test]
		public void CheckPassword_ShouldReturnTrue_WhenPasswordsMatch()
		{
			// Arrange
			var password = "password";
			var hmac = new HMACSHA512();
			var user = new User
			{
				PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password)),
				PasswordSalt = hmac.Key
			};

			// Act
			var result = _userService.CheckPassword(password, user);

			// Assert
			result.Should().BeTrue();
		}

		[Test]
		public void CheckPassword_ShouldReturnFalse_WhenPasswordsDoNotMatch()
		{
			// Arrange
			var password = "password";
			var hmac = new HMACSHA512();
			var user = new User
			{
				PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("wrongpassword")),
				PasswordSalt = hmac.Key
			};

			// Act
			var result = _userService.CheckPassword(password, user);

			// Assert
			result.Should().BeFalse();
		}
	}
}
