using Api.Extensions;
using Api.Helpers.Errors;
using Application.Abstractions;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// 🔥 REEMPLAZAR AddOpenApi() por Swagger completo
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Tu API - Sistema de Gestión",
        Version = "v1",
        Description = "API para gestión de clientes, vehículos y órdenes de servicio",
        Contact = new OpenApiContact
        {
            Name = "Tu Equipo",
            Email = "soporte@tuempresa.com"
        }
    });

    // 🔐 Configurar seguridad JWT Bearer
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n" +
                      "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n" +
                      "Example: \"Bearer 12345abcdef\""
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // 📝 Incluir comentarios XML de los controllers
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }

    // 🏷️ Habilitar anotaciones Swagger
    options.EnableAnnotations();

    // 📊 Ordenar endpoints alfabéticamente
    options.OrderActionsBy(apiDesc => $"{apiDesc.ActionDescriptor.RouteValues["controller"]}_{apiDesc.HttpMethod}");
});

// Tus configuraciones existentes
builder.Services.ConfigureCors();
builder.Services.AddCustomRateLimiter();
builder.Services.AddJwt(builder.Configuration);
builder.Services.AddApplicationServices();

// Configuración de DbContext (simplificada - veo que tienes duplicada)
var configuration = builder.Configuration;

builder.Services.AddDbContext<AppDbContext>(options =>
{
    var conn = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true"
        ? configuration.GetConnectionString("Postgres")       // cuando está en Docker
        : configuration.GetConnectionString("PostgresLocal"); // cuando está local
    options.UseNpgsql(conn);
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

// Tus repositorios existentes
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IDetalleOrdenRepository, DetalleOrdenRepository>();
builder.Services.AddScoped<IFacturaRepository, FacturaRepository>();
builder.Services.AddScoped<IMarcaRepository, MarcaRepository>();
builder.Services.AddScoped<IModeloRepository, ModeloRepository>();
builder.Services.AddScoped<IOrdenServicioRepository, OrdenServicioRepository>();
builder.Services.AddScoped<IPagoRepository, PagoRepository>();
builder.Services.AddScoped<IRepuestoRepository, RepuestoRepository>();
builder.Services.AddScoped<IVehiculoRepository, VehiculoRepository>();
builder.Services.AddScoped<ICitaRepository, CitaRepository>();
builder.Services.AddScoped<IAuditoriaRepository, AuditoriaRepository>();

var app = builder.Build();

// 🔥 MIDDLEWARE DE SWAGGER (siempre disponible, no solo en Development)
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Tu API v1");
    options.RoutePrefix = "swagger"; // Acceder en: /swagger
    options.DocumentTitle = "Documentación API - Sistema Gestión";
    options.EnablePersistAuthorization(); // Persistir token entre sesiones
    options.EnableFilter(); // Habilitar búsqueda/filtrado
});

// Tus middlewares existentes
app.UseMiddleware<ExceptionMiddleware>();
await app.SeedRolesAsync();

// Configure the HTTP request pipeline.
// ❌ ELIMINAR esto ya que ahora usamos Swagger UI completo
// if (app.Environment.IsDevelopment())
// {
//     app.MapOpenApi();
// }

app.UseCors("CorsPolicy");
app.UseCors("CorsPolicyUrl");
app.UseCors("Dinamica");

// ⚠️ CORREGIR: El orden correcto es Authentication -> Authorization
app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();
app.UseRateLimiter();

app.MapControllers();

// Migración de base de datos
using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
db.Database.Migrate();

app.Run();