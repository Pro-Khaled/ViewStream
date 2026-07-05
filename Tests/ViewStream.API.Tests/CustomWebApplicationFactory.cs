using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Testcontainers.RabbitMq;
using Testcontainers.Redis;
using Testcontainers.SqlEdge;
using ViewStream.Application.Interfaces.Services;

namespace ViewStream.API.Tests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        public SqlEdgeContainer SqlContainer { get; private set; } = null!;
        public RedisContainer RedisContainer { get; private set; } = null!;
        public RabbitMqContainer RabbitContainer { get; private set; } = null!;

        public async Task InitializeAsync()
        {
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

            await Task.WhenAll(
                SqlContainer.StartAsync(),
                RedisContainer.StartAsync(),
                RabbitContainer.StartAsync()
            );

            // Run migrations to prepare schema
            var optionsBuilder = new Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<ViewStream.Infrastructure.Persistence.ViewStreamDbContext>();
            optionsBuilder.UseSqlServer(SqlContainer.GetConnectionString());
            using var context = new ViewStream.Infrastructure.Persistence.ViewStreamDbContext(optionsBuilder.Options);
            await context.Database.MigrateAsync();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // Override configuration connection strings with Testcontainers values
            builder.ConfigureAppConfiguration((context, configBuilder) =>
            {
                configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "ConnectionStrings:DefaultConnection", SqlContainer.GetConnectionString() },
                    { "Redis:ConnectionString", RedisContainer.GetConnectionString() },
                    { "RabbitMQ:Host", RabbitContainer.Hostname },
                    { "RabbitMQ:Port", RabbitContainer.GetMappedPublicPort(5672).ToString() },
                    { "RabbitMQ:Username", "guest" },
                    { "RabbitMQ:Password", "guest" }
                });
            });

            builder.ConfigureTestServices(services =>
            {
                // Replace external services with mocks to avoid network side-effects
                services.RemoveAll<IEmailService>();
                services.AddSingleton(Mock.Of<IEmailService>());

                services.RemoveAll<IPushNotificationService>();
                services.AddSingleton(Mock.Of<IPushNotificationService>());
            });
        }

        public new async Task DisposeAsync()
        {
            await Task.WhenAll(
                SqlContainer.DisposeAsync().AsTask(),
                RedisContainer.DisposeAsync().AsTask(),
                RabbitContainer.DisposeAsync().AsTask()
            );
            await base.DisposeAsync();
        }
    }
}