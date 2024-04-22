using FlightsApp.Controllers;
using FlightsApp.Data;
using FlightsApp.Dtos;
using FlightsApp.Models;
using FlightsApp.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Xunit;

namespace FlightsApp.Tests
{
    public class AuthControllerTests
    {
        public enum UserManagerBehavior
        {
            RegisterSuccess,
            UserExists,
            WeakPassword,
            ValidLogin, 
            InvalidLogin,
            Default
        }

        public static Mock<UserManager<UserModel>> MockUserManager(List<UserModel> ls, UserManagerBehavior behavior)
        {
            var store = new Mock<IUserStore<UserModel>>();
            var mgr = new Mock<UserManager<UserModel>>(store.Object, null, null, null, null, null, null, null, null);
            mgr.Object.UserValidators.Add(new UserValidator<UserModel>());
            mgr.Object.PasswordValidators.Add(new PasswordValidator<UserModel>());

            mgr.Setup(x => x.DeleteAsync(It.IsAny<UserModel>())).ReturnsAsync(IdentityResult.Success);
            mgr.Setup(x => x.CreateAsync(It.IsAny<UserModel>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success).Callback<UserModel, string>((x, y) => ls.Add(x));
            mgr.Setup(x => x.UpdateAsync(It.IsAny<UserModel>())).ReturnsAsync(IdentityResult.Success);

            switch (behavior)
            {
                case UserManagerBehavior.RegisterSuccess:
                    mgr.Setup(x => x.CreateAsync(It.IsAny<UserModel>(), It.IsAny<string>()))
                       .ReturnsAsync(IdentityResult.Success)
                       .Callback<UserModel, string>((x, y) => ls.Add(x));
                    break;
                case UserManagerBehavior.UserExists:
                    mgr.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                       .ReturnsAsync((string username) => ls.FirstOrDefault(u => (u as UserModel)?.UserName.Length>0));
                    break;
                case UserManagerBehavior.WeakPassword:
                    mgr.Setup(x => x.CreateAsync(It.IsAny<UserModel>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed(new IdentityError { Code = "PasswordMismatch" }));
                    break;
                case UserManagerBehavior.ValidLogin:
                    UserModel user = new UserModel { UserName = "testUser" };
                    mgr.Setup(x => x.FindByNameAsync("testUser")).ReturnsAsync(user);
                    mgr.Setup(x => x.CheckPasswordAsync(user, "testPassword")).ReturnsAsync(true);
                    break;
                case UserManagerBehavior.InvalidLogin:
                    mgr.Setup(x => x.FindByNameAsync("testUser")).ReturnsAsync((UserModel)null);
                    break;
                default:
                    mgr.Setup(x => x.CreateAsync(It.IsAny<UserModel>(), It.IsAny<string>()))
                       .ReturnsAsync(IdentityResult.Success)
                       .Callback<UserModel, string>((x, y) => ls.Add(x));
                    mgr.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                       .ReturnsAsync((string username) => ls.FirstOrDefault(u => u.UserName == username));
                    break;
            }

            return mgr;
        }

        private List<UserModel> _users = new List<UserModel>
        {
            new UserModel { UserName = "user1", Email = "test@example.com",
                },
            new UserModel { UserName = "user2", Email = "test@example.com",
                },
        };

        private AuthController _authController;
        private UserManager<UserModel> _userManager;
        private readonly IConfiguration _configuration;

        public AuthControllerTests()
        {
            var configBuilder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            _configuration = configBuilder.Build();

            _userManager = MockUserManager(_users, UserManagerBehavior.Default).Object;
            var _authRepository = new AuthRepository(_userManager, _configuration);
            _authController = new AuthController(_authRepository);
        }

        [Fact]
        public async Task Register_ShouldRegisterUser()
        {
            // Arrange
            var registerModel = new RegisterDto
            {
                UserName = "testUser",
                Email = "test@example.com",
                Password = "P@ssw0rd"
            };

            // Act
            _userManager = MockUserManager(_users, UserManagerBehavior.RegisterSuccess).Object;
            var _authRepository = new AuthRepository(_userManager, _configuration);
            _authController = new AuthController(_authRepository);
            var result = await _authController.Register(registerModel);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<AuthResponse>(okResult.Value);
            Assert.Equal("Success", response.Status);
            Assert.Equal("User created successfully.", response.Message);
        }

        [Fact]
        public async Task Register_ExistingUsername_ReturnsBadRequest()
        {
            // Arrange
            var registerModel = new RegisterDto
            {
                UserName = "testUser",
                Email = "test@example.com",
                Password = "P@ssw0rd"
            };

            // Act
            _userManager = MockUserManager(_users, UserManagerBehavior.UserExists).Object;
            var _authRepository = new AuthRepository(_userManager, _configuration);
            _authController = new AuthController(_authRepository);
            var result = await _authController.Register(registerModel);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<AuthResponse>(badRequestResult.Value);
            Assert.Equal("Error", response.Status);
            Assert.Equal("User already exists", response.Message);
        }

        [Fact]
        public async Task Register_WeakPassword_ReturnsBadRequest()
        {
            // Arrange
            var registerModel = new RegisterDto
            {
                UserName = "testUser",
                Email = "test@example.com",
                Password = "abc"
            };

            // Act
            _userManager = MockUserManager(_users, UserManagerBehavior.WeakPassword).Object;
            var _authRepository = new AuthRepository(_userManager, _configuration);
            _authController = new AuthController(_authRepository);
            var result = await _authController.Register(registerModel);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<AuthResponse>(badRequestResult.Value);
            Assert.Equal("Error", response.Status);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsToken()
        {
            // Arrange
            var loginModel = new LoginDto
            {
                UserName = "testUser",
                Password = "testPassword"
            };

            _userManager = MockUserManager(_users, UserManagerBehavior.ValidLogin).Object;

            var _authRepository = new AuthRepository(_userManager, _configuration);
            _authController = new AuthController(_authRepository);

            // Act
            var result = await _authController.Login(loginModel);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic response = okResult.Value;
            Assert.NotNull(response.token);
            Assert.IsType<string>(response.token);
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var loginModel = new LoginDto
            {
                UserName = "testUser",
                Password = "testPassword"
            };

            _userManager = MockUserManager(_users, UserManagerBehavior.InvalidLogin).Object;
            var _authRepository = new AuthRepository(_userManager, _configuration);
            _authController = new AuthController(_authRepository);

            // Act
            var result = await _authController.Login(loginModel);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }
    }
}
