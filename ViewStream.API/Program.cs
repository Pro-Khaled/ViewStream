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
        .ReadFrom.Configuration(context.Configuration)   // reads appsettings.json
        .Enrich.FromLogContext()
        .WriteTo.Console()
);

// Add API services
builder.Services.AddApi(builder.Configuration);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
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
    app.UseApi(app.Configuration); // This sets up Swagger + JWT
}

//Register the custom exception handling middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();


//app.UseAuthorization();

//app.UseStaticFiles(); // Serves files from wwwroot

// Optionally, if you want to serve from a custom physical folder outside wwwroot:
//app.UseStaticFiles(new StaticFileOptions
//{
//    FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath, "Uploads")),
//    RequestPath = "/uploads"
//});

//  Temporary test endpoint – place it here
//app.MapGet("/config-test", (IOptions<JwtOptions> opts) =>
//    new { opts.Value.Key, opts.Value.Issuer, opts.Value.Audience });

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath, "uploads")),
    RequestPath = "/uploads"
});

app.MapControllers();
app.MapHub<EpisodeHub>("/hubs/episode");
app.MapHub<ShowHub>("/hubs/show");
app.MapHub<NotificationHub>("/hubs/notification");

// Health Checks endpoint
app.MapHealthChecks("/health");

RecurringJob.AddOrUpdate<NotificationRetryJob>(
    "notification-retry",
    job => job.Execute(),
    Cron.Hourly);

app.Run();
