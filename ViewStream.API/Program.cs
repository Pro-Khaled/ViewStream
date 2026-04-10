using Microsoft.Extensions.Options;
using ViewStream.API;
using ViewStream.Infrastructure.Seeding;
using ViewStream.Shared.Options;
var builder = WebApplication.CreateBuilder(args);

// Add API services
builder.Services.AddApi(builder.Configuration);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

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

app.UseHttpsRedirection();


app.UseAuthorization();

//  Temporary test endpoint – place it here
//app.MapGet("/config-test", (IOptions<JwtOptions> opts) =>
//    new { opts.Value.Key, opts.Value.Issuer, opts.Value.Audience });

app.MapControllers();

app.Run();
