using Api.Extensions;
using Api.Helpers.Errors;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.ConfigureCors();
builder.Services.AddCustomRateLimiter();
builder.Services.AddJwt(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    string connectionString = builder.Configuration.GetConnectionString("PostgresLocal")!;
    options.UseNpgsql(connectionString);
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

// builder.Services.AddDbContext<AppDbContext>(opt =>
//     opt.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

var app = builder.Build();
app.UseMiddleware<ExceptionMiddleware>();
await app.SeedRolesAsync();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseCors("CorsPolicy");
app.UseCors("CorsPolicyUrl");
app.UseCors("Dinamica");

app.UseHttpsRedirection();
app.UseRateLimiter();

app.UseHttpsRedirection();
app.UseRateLimiter();
app.UseAuthorization();
app.UseAuthentication();

app.MapControllers();

using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
db.Database.Migrate();


app.Run();
