using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Infrastructure;
using ViewStream.Shared.Options;
using ViewStream.VideoProcessor.Consumers;
using ViewStream.VideoProcessor.Services;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        // Add shared options (required for Infrastructure)
        services.Configure<DatabaseConnectionOptions>(
            hostContext.Configuration.GetSection(DatabaseConnectionOptions.SectionName));
        services.Configure<DatabaseOptions>(
            hostContext.Configuration.GetSection(DatabaseOptions.SectionName));
        services.Configure<RabbitMQOptions>(
            hostContext.Configuration.GetSection(RabbitMQOptions.SectionName));
        services.Configure<StorageOptions>(
            hostContext.Configuration.GetSection("Storage"));

        // Add Infrastructure (Db, repositories, unit of work, file storage)
        services.AddInfrastructure(hostContext.Configuration);

        // Register custom storage options for worker
        services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<StorageOptions>>().Value);

        // Register FFmpeg encoder
        services.AddSingleton<IVideoEncoder, FfmpegVideoEncoder>();

        // Firebase push service
        services.AddSingleton<IPushNotificationService, FirebasePushService>();

        // Register HLS profile configuration
        services.Configure<List<HlsProfile>>(
            hostContext.Configuration.GetSection("HlsProfiles"));

        // MassTransit + RabbitMQ consumer
        services.AddMassTransit(x =>
        {
            x.AddConsumer<TranscodeVideoConsumer>();
            x.AddConsumer<NotificationConsumer>();  


            x.UsingRabbitMq((ctx, cfg) =>
            {
                var rabbitOptions = ctx.GetRequiredService<IOptions<RabbitMQOptions>>().Value;
                cfg.Host(rabbitOptions.Host, "/", h =>
                {
                    h.Username(rabbitOptions.Username);
                    h.Password(rabbitOptions.Password);
                });

                cfg.ReceiveEndpoint("transcode-video-queue", e =>
                {
                    e.ConfigureConsumer<TranscodeVideoConsumer>(ctx);
                });
                cfg.ReceiveEndpoint("notification-queue", e =>
                {
                    e.ConfigureConsumer<NotificationConsumer>(ctx);
                });
            });
        });
    })
    .Build();

await host.RunAsync();