using LibraryManagementSystem.Infrastructure;
using LibraryManagementSystem.Infrastructure.Data.Seed;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// 1. Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog(); // Replaces the default logger

// 2. Register Infrastructure (DbContext, Identity, Repositories)
builder.Services.AddInfrastructure(builder.Configuration);

// 3. Register Application Services (Business Logic)
builder.Services.AddApplicationServices();

// 4. Configure MVC / Razor Pages
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<LibraryManagementSystem.Web.Filters.AuditActionFilter>();
});
builder.Services.AddRazorPages();

var app = builder.Build();

// 5. Seed Database on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await DataSeeder.SeedAsync(services);
    }
    catch (Exception ex)
    {
        Log.Error(ex, "An error occurred while seeding the database.");
    }
}

// 6. Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Add Serilog Request Logging Middleware
app.UseSerilogRequestLogging();

// Add Global Exception Middleware
app.UseMiddleware<LibraryManagementSystem.Web.Middleware.GlobalExceptionHandlerMiddleware>();

// Handle 404 and 403 errors gracefully
app.UseStatusCodePagesWithRedirects("/Home/StatusCode?code={0}");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

try
{
    Log.Information("Starting web application");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}