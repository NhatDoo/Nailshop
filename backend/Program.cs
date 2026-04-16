using Microsoft.EntityFrameworkCore;
using backend.context.identity;
using backend.context.common.infrastructure.interceptors;
using backend.context.common.api.middlewares;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<DomainEventInterceptor>();

builder.Services.AddDbContext<AppDbContext>((sp, options) =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
           .AddInterceptors(sp.GetRequiredService<DomainEventInterceptor>());
});
builder.Services.AddIdentityModule();
builder.Services.AddControllers();
var app = builder.Build();
app.UseMiddleware<ExceptionMiddleware>();
app.MapControllers();
app.UseHttpsRedirection();
app.MapGet("/test-db", async (AppDbContext dbContext) =>
{
    try
    {
        var canConnect = await dbContext.Database.CanConnectAsync();
        return canConnect 
            ? Results.Ok(new { status = "success", message = "Database connection successful!" }) 
            : Results.Problem("Cannot connect to database.");
    }
    catch (Exception ex)
    {
        return Results.Problem($"Database connection failed: {ex.Message}");
    }
})
.WithName("TestDbConnection");

app.Run();
