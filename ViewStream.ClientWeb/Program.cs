var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Serve static files from wwwroot 
app.UseDefaultFiles();
app.UseStaticFiles();

// Optional: Fallback to index.html for SPA-style routing
app.MapFallbackToFile("index.html");

//// Point to the Vite production build folder (wwwroot1/dist)
//var vueDistPath = Path.Combine(builder.Environment.ContentRootPath, "wwwroot1", "dist");

//// Ensure the directory exists to avoid startup errors if not built yet
//if (!Directory.Exists(vueDistPath))
//{
//    Directory.CreateDirectory(vueDistPath);
//}

//var fileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(vueDistPath);

//app.UseDefaultFiles(new DefaultFilesOptions
//{
//    FileProvider = fileProvider
//});

//app.UseStaticFiles(new StaticFileOptions
//{
//    FileProvider = fileProvider
//});

//// Fallback to index.html for SPA-style routing (Vue Router)
//app.MapFallbackToFile("index.html", new StaticFileOptions
//{
//    FileProvider = fileProvider
//});

app.Run();