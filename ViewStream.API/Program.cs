using Hangfire;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Serilog;
using ViewStream.Api.Hubs;
using ViewStream.Api.Middleware;
using ViewStream.API;
using ViewStream.API.Hubs;
using ViewStream.API.Jobs;
using ViewStream.Infrastructure.Seeding;
using ViewStream.Shared.Options;
var builder = WebApplication.CreateBuilder(args);

//  Configure Serilog host
builder.Host.UseSerilog((context, services, configuration) =>
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console()
);

// Add API services
builder.Services.AddApi(builder.Configuration);

builder.Services.AddOpenApi();

var app = builder.Build();

// Use Serilog request logging middleware
app.UseSerilogRequestLogging();

// Seed roles and SupperAdminAccount on startup
using (var scope = app.Services.CreateScope())
{
    await RoleSeeder.SeedRolesAsync(scope.ServiceProvider);
    await AdminSeeder.SeedAdminAsync(scope.ServiceProvider);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseApi(app.Configuration);
}

//Register the custom exception handling middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath, "uploads")),
    RequestPath = "/uploads"
});

// Phase 1: Geo-location middleware (resolves IP to country code)
app.UseMiddleware<ViewStream.API.Middleware.GeoLocationMiddleware>();

// Phase 3: Block API access for users with pending/approved data deletion
app.UseMiddleware<ViewStream.API.Middleware.DataDeletionBlockMiddleware>();

app.MapControllers();
app.MapHub<EpisodeHub>("/hubs/episode");
app.MapHub<ShowHub>("/hubs/show");
app.MapHub<NotificationHub>("/hubs/notification");
app.MapHub<WatchPartyHub>("/hubs/watchparty");
app.MapHub<AdminNotificationHub>("/hubs/admin-notifications");

// Health Checks endpoint
app.MapHealthChecks("/health");

RecurringJob.AddOrUpdate<NotificationRetryJob>(
    "notification-retry",
    job => job.Execute(),
    Cron.Hourly);

// Register all recurring Hangfire jobs (Phases 2-9)
HangfireJobRegistration.RegisterAllRecurringJobs();

app.Run();