//Set up builder for webapp
var builder = WebApplication.CreateBuilder(args);
//Build the builder
var app = builder.Build();

//Middleware - UseDefault: enables default filemapping through wwwroot folder
app.UseDefaultFiles();
//Use static allows server to get associated files through the wwwroot folder
app.UseStaticFiles();

//API-endpoint to check server health
app.MapGet("/health", () => "Server status: OK");

//Start the server
app.Run();
