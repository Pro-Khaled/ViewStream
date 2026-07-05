using Microsoft.EntityFrameworkCore;
using Testcontainers.RabbitMq;
using Testcontainers.Redis;
using Testcontainers.SqlEdge;
using ViewStream.Infrastructure.Persistence;

namespace ViewStream.Infrastructure.Tests
{
    public class DatabaseFixture : IAsyncLifetime
    {
        public SqlEdgeContainer SqlContainer { get; private set; } = null!;
        public RedisContainer RedisContainer { get; private set; } = null!;
        public RabbitMqContainer RabbitContainer { get; private set; } = null!;

        public string DbConnectionString => SqlContainer.GetConnectionString();
        public string RedisConnectionString => RedisContainer.GetConnectionString();
        public string RabbitConnectionString => RabbitContainer.GetConnectionString();

        public async Task InitializeAsync()
        {
            // Use Azure SQL Edge container for MS SQL Server compatibility
            SqlContainer = new SqlEdgeBuilder()
                .WithImage("mcr.microsoft.com/azure-sql-edge:latest")
                .WithPassword("SecurePassword123!")
                .Build();

            RedisContainer = new RedisBuilder()
                .WithImage("redis:alpine")
                .Build();

            RabbitContainer = new RabbitMqBuilder("rabbitmq:3-management-alpine")
                .WithUsername("guest")
                .WithPassword("guest")
                .Build();

            // Start all containers in parallel
            await Task.WhenAll(
                SqlContainer.StartAsync(),
                RedisContainer.StartAsync(),
                RabbitContainer.StartAsync()
            );

            // Run migrations on SQL Server container database to prepare the schema
            var optionsBuilder = new DbContextOptionsBuilder<ViewStreamDbContext>();
            optionsBuilder.UseSqlServer(DbConnectionString);

            using var context = new ViewStreamDbContext(optionsBuilder.Options);
            await context.Database.MigrateAsync();
        }

        public async Task DisposeAsync()
        {
            await Task.WhenAll(
                SqlContainer.DisposeAsync().AsTask(),
                RedisContainer.DisposeAsync().AsTask(),
                RabbitContainer.DisposeAsync().AsTask()
            );
        }
    }
}