using FlightsApp.Controllers;
using FlightsApp.Data;
using FlightsApp.Models;
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

        public static Mock<UserManager<User>> MockUserManager(List<User> ls, UserManagerBehavior behavior)
        {
            var store = new Mock<IUserStore<User>>();
            var mgr = new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);
            mgr.Object.UserValidators.Add(new UserValidator<User>());
            mgr.Object.PasswordValidators.Add(new PasswordValidator<User>());

            mgr.Setup(x => x.DeleteAsync(It.IsAny<User>())).ReturnsAsync(IdentityResult.Success);
            mgr.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success).Callback<User, string>((x, y) => ls.Add(x));
            mgr.Setup(x => x.UpdateAsync(It.IsAny<User>())).ReturnsAsync(IdentityResult.Success);

            switch (behavior)
            {
                case UserManagerBehavior.RegisterSuccess:
                    mgr.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                       .ReturnsAsync(IdentityResult.Success)
                       .Callback<User, string>((x, y) => ls.Add(x));
                    break;
                case UserManagerBehavior.UserExists:
                    mgr.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                       .ReturnsAsync((string username) => ls.FirstOrDefault(u => (u as User)?.UserName.Length>0));
                    break;
                case UserManagerBehavior.WeakPassword:
                    mgr.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed(new IdentityError { Code = "PasswordMismatch" }));
                    break;
                case UserManagerBehavior.ValidLogin:
                    User user = new User { UserName = "testUser" };
                    mgr.Setup(x => x.FindByNameAsync("testUser")).ReturnsAsync(user);
                    mgr.Setup(x => x.CheckPasswordAsync(user, "testPassword")).ReturnsAsync(true);
                    break;
                case UserManagerBehavior.InvalidLogin:
                    mgr.Setup(x => x.FindByNameAsync("testUser")).ReturnsAsync((User)null);
                    break;
                default:
                    mgr.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                       .ReturnsAsync(IdentityResult.Success)
                       .Callback<User, string>((x, y) => ls.Add(x));
                    mgr.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                       .ReturnsAsync((string username) => ls.FirstOrDefault(u => u.UserName == username));
                    break;
            }

            return mgr;
        }

        private List<User> _users = new List<User>
        {
            new User { UserName = "user1", Email = "test@example.com",
                },
            new User { UserName = "user2", Email = "test@example.com",
                },
        };

        private AuthController _authController;
        private UserManager<User> _userManager;
        private readonly IConfiguration _configuration;

        public AuthControllerTests()
        {
            var configBuilder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            _configuration = configBuilder.Build();

            _userManager = MockUserManager(_users, UserManagerBehavior.Default).Object;
            _authController = new AuthController(_userManager, _configuration);
        }

        [Fact]
        public async Task Register_ShouldRegisterUser()
        {
            // Arrange
            var registerModel = new RegisterModel
            {
                UserName = "testUser",
                Email = "test@example.com",
                Password = "P@ssw0rd"
            };

            // Act
            _userManager = MockUserManager(_users, UserManagerBehavior.RegisterSuccess).Object;
            _authController = new AuthController(_userManager, _configuration);
            var result = await _authController.Register(registerModel);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response>(okResult.Value);
            Assert.Equal("Success", response.Status);
            Assert.Equal("User created successfully", response.Message);
        }

        [Fact]
        public async Task Register_ExistingUsername_ReturnsBadRequest()
        {
            // Arrange
            var registerModel = new RegisterModel
            {
                UserName = "testUser",
                Email = "test@example.com",
                Password = "P@ssw0rd"
            };

            // Act
            _userManager = MockUserManager(_users, UserManagerBehavior.UserExists).Object;
            _authController = new AuthController(_userManager, _configuration);
            var result = await _authController.Register(registerModel);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<Response>(badRequestResult.Value);
            Assert.Equal("Error", response.Status);
            Assert.Equal("User already exists", response.Message);
        }

        [Fact]
        public async Task Register_WeakPassword_ReturnsBadRequest()
        {
            // Arrange
            var registerModel = new RegisterModel
            {
                UserName = "testUser",
                Email = "test@example.com",
                Password = "abc"
            };

            // Act
            _userManager = MockUserManager(_users, UserManagerBehavior.WeakPassword).Object;
            _authController = new AuthController(_userManager, _configuration);
            var result = await _authController.Register(registerModel);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<Response>(badRequestResult.Value);
            Assert.Equal("Error", response.Status);
            Assert.Equal("Password must contain at least one digit, one lowercase letter,one uppercase letter,one non - alphanumeric character,and be at least 8 characters long.", response.Message);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsToken()
        {
            // Arrange
            var loginModel = new LoginModel
            {
                UserName = "testUser",
                Password = "testPassword"
            };

            _userManager = MockUserManager(_users, UserManagerBehavior.ValidLogin).Object;

            _authController = new AuthController(_userManager, _configuration);

            // Act
            var result = await _authController.Login(loginModel);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic response = okResult.Value;
            Assert.NotNull(response.token);
            Assert.NotNull(response.expiration);
            Assert.IsType<string>(response.token);
            Assert.IsType<DateTime>(response.expiration);
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var loginModel = new LoginModel
            {
                UserName = "testUser",
                Password = "testPassword"
            };

            _userManager = MockUserManager(_users, UserManagerBehavior.InvalidLogin).Object;
            _authController = new AuthController(_userManager, _configuration);

            // Act
            var result = await _authController.Login(loginModel);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }
    }
}
