using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Respawn;
using ViewStream.Application.DTOs.Account;
using ViewStream.Infrastructure.Persistence;
using Xunit;

namespace ViewStream.API.Tests
{
    [CollectionDefinition("API Collection")]
    public class ApiCollection : ICollectionFixture<CustomWebApplicationFactory>
    {
    }

    [Collection("API Collection")]
    public abstract class ApiTestBase : IAsyncLifetime
    {
        protected readonly CustomWebApplicationFactory Factory;
        protected HttpClient Client { get; private set; } = null!;
        private SqlConnection _connection = null!;
        private Respawner _respawner = null!;

        protected ApiTestBase(CustomWebApplicationFactory factory)
        {
            Factory = factory;
        }

        public async Task InitializeAsync()
        {
            Client = Factory.CreateClient();

            _connection = new SqlConnection(Factory.SqlContainer.GetConnectionString());
            await _connection.OpenAsync();

            _respawner = await Respawner.CreateAsync(_connection, new RespawnerOptions
            {
                DbAdapter = DbAdapter.SqlServer,
                TablesToIgnore = new Respawn.Graph.Table[] { "__EFMigrationsHistory" }
            });

            await _respawner.ResetAsync(_connection);
        }

        public async Task DisposeAsync()
        {
            if (_connection != null)
            {
                await _connection.CloseAsync();
                await _connection.DisposeAsync();
            }

            Client.Dispose();
        }

        protected async Task ConfirmEmailInDatabaseAsync(string email)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ViewStreamDbContext>();
            optionsBuilder.UseSqlServer(Factory.SqlContainer.GetConnectionString());
            using var context = new ViewStreamDbContext(optionsBuilder.Options);
            
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user != null)
            {
                user.EmailConfirmed = true;
                context.Users.Update(user);
                await context.SaveChangesAsync();
            }
        }

        protected async Task AuthenticateAsync(string email = "testuser@example.com", string password = "Password123!")
        {
            // Register user
            var registerDto = new RegisterDto
            {
                Email = email,
                Password = password,
                ConfirmPassword = password,
                FullName = "Test User",
                CountryCode = "US"
            };
            
            await Client.PostAsJsonAsync("api/v1/Account/register", registerDto);

            // Directly confirm the email in the DB
            await ConfirmEmailInDatabaseAsync(email);
            
            // Login to get token
            var loginDto = new LoginDto
            {
                Email = email,
                Password = password
            };

            var loginResponse = await Client.PostAsJsonAsync("api/v1/Account/login", loginDto);
            if (loginResponse.IsSuccessStatusCode)
            {
                var authResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponseDto>();
                if (authResult != null)
                {
                    Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResult.AccessToken);
                }
            }
        }
    }
}
