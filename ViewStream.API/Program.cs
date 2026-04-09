using ViewStream.API;
var builder = WebApplication.CreateBuilder(args);

// Add API services
builder.Services.AddApi(builder.Configuration);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseApi(app.Configuration); // This sets up Swagger + JWT
}

app.UseHttpsRedirection();


app.UseAuthorization();

app.MapControllers();

app.Run();
