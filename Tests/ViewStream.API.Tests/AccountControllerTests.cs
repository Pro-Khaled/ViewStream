using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using ViewStream.Application.DTOs.Account;
using Xunit;

namespace ViewStream.API.Tests
{
    public class AccountControllerTests : ApiTestBase
    {
        public AccountControllerTests(CustomWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task Register_WithValidDto_ShouldReturnSuccess()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Email = "newuser@example.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                FullName = "New User",
                CountryCode = "US"
            };

            // Act
            var response = await Client.PostAsJsonAsync("api/v1/Account/register", registerDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Login_WithValidCredentials_ShouldReturnTokens()
        {
            // Arrange
            var email = "loginuser@example.com";
            var password = "Password123!";
            
            var registerDto = new RegisterDto
            {
                Email = email,
                Password = password,
                ConfirmPassword = password,
                FullName = "Login User",
                CountryCode = "US"
            };
            await Client.PostAsJsonAsync("api/v1/Account/register", registerDto);
            await ConfirmEmailInDatabaseAsync(email);

            var loginDto = new LoginDto
            {
                Email = email,
                Password = password
            };

            // Act
            var response = await Client.PostAsJsonAsync("api/v1/Account/login", loginDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
            result.Should().NotBeNull();
            result!.AccessToken.Should().NotBeNullOrEmpty();
            result.RefreshToken.Should().NotBeNullOrEmpty();
            result.User.Email.Should().Be(email);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ShouldReturnUnauthorized()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "nonexistent@example.com",
                Password = "WrongPassword123!"
            };

            // Act
            var response = await Client.PostAsJsonAsync("api/v1/Account/login", loginDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task AccessSecureEndpoint_WithoutJwt_ShouldReturnUnauthorized()
        {
            // Act
            var response = await Client.GetAsync("api/v1/Episodes/1/stream");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
