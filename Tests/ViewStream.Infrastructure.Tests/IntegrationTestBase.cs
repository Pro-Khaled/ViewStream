using System.Data.Common;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Respawn;
using ViewStream.Infrastructure.Persistence;

namespace ViewStream.Infrastructure.Tests
{
    [CollectionDefinition("Database collection")]
    public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
    {
    }

    [Collection("Database collection")]
    public abstract class IntegrationTestBase : IAsyncLifetime
    {
        protected readonly DatabaseFixture Fixture;
        private SqlConnection _connection = null!;
        private Respawner _respawner = null!;

        protected ViewStreamDbContext DbContext { get; private set; } = null!;

        protected IntegrationTestBase(DatabaseFixture fixture)
        {
            Fixture = fixture;
        }

        public async Task InitializeAsync()
        {
            _connection = new SqlConnection(Fixture.DbConnectionString);
            await _connection.OpenAsync();

            _respawner = await Respawner.CreateAsync(_connection, new RespawnerOptions
            {
                DbAdapter = DbAdapter.SqlServer,
                TablesToIgnore = new Respawn.Graph.Table[] { "__EFMigrationsHistory" }
            });

            await _respawner.ResetAsync(_connection);

            var optionsBuilder = new DbContextOptionsBuilder<ViewStreamDbContext>();
            optionsBuilder.UseSqlServer(Fixture.DbConnectionString);
            DbContext = new ViewStreamDbContext(optionsBuilder.Options);
        }

        public async Task DisposeAsync()
        {
            if (DbContext != null)
            {
                await DbContext.DisposeAsync();
            }

            if (_connection != null)
            {
                await _connection.CloseAsync();
                await _connection.DisposeAsync();
            }
        }
    }
}
