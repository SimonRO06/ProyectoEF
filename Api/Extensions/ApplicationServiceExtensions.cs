using System;
using System.Text;
using System.Threading.RateLimiting;
using Api.Helpers;
using Api.Helpers.Errors;
using Api.Mapping;
using Api.Services;
using Application.Abstractions;
using Domain.Entities.Auth;
using FluentValidation;
using Infrastructure.Persistence.Repositories;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Api.Extensions;
public static class ApplicationServiceExtensions
{
    public static void ConfigureCors(this IServiceCollection services) =>

        services.AddCors(options =>
        {
            HashSet<String> allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "https://app.ejemplo.com",
                "https://admin.ejemplo.com"
            };
            options.AddPolicy("CorsPolicy", builder =>
                builder.AllowAnyOrigin()   //WithOrigins("https://dominio.com")
                .AllowAnyMethod()          //WithMethods("GET","POST")
                .AllowAnyHeader());        //WithHeaders("accept","content-type")

            options.AddPolicy("CorsPolicyUrl", builder =>
                builder.WithOrigins("https://localhost:4200", "https://localhost:5500")   //WithOrigins("https://dominio.com")
                .AllowAnyMethod()          //WithMethods("GET","POST")
                .AllowAnyHeader());

            options.AddPolicy("Dinamica", builder =>
                builder.SetIsOriginAllowed(origin => allowed.Contains(origin))   //WithOrigins("https://dominio.com")
                .WithMethods("GET", "POST")
                .WithHeaders("Content-Type", "Authorization"));        //WithHeaders("accept","content-type")
        });
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IPasswordHasher<UserMember>, PasswordHasher<UserMember>>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IClienteRepository, ClienteRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
        services.AddValidatorsFromAssembly(typeof(Program).Assembly);
        services.AddAutoMapper(typeof(Program).Assembly);
        services.AddAutoMapper(typeof(ClienteProfile).Assembly);
        services.AddAutoMapper(typeof(DetalleOrdenProfile).Assembly);
        services.AddAutoMapper(typeof(FacturaProfile).Assembly);
        services.AddAutoMapper(typeof(MarcaProfile).Assembly);
        services.AddAutoMapper(typeof(ModeloProfile).Assembly);
        services.AddAutoMapper(typeof(OrdenServicioProfile).Assembly);
        services.AddAutoMapper(typeof(PagoProfile).Assembly);
        services.AddAutoMapper(typeof(RepuestoProfile).Assembly);
        services.AddAutoMapper(typeof(VehiculoProfile).Assembly);
    }
    public static IServiceCollection AddCustomRateLimiter(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.OnRejected = async (context, token) =>
            {
                var ip = context.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "desconocida";
                context.HttpContext.Response.StatusCode = 429;
                context.HttpContext.Response.ContentType = "application/json";
                var mensaje = $"{{\"message\": \"Demasiadas peticiones desde la IP {ip}. Intenta más tarde.\"}}";
                await context.HttpContext.Response.WriteAsync(mensaje, token);
            };

            // Aquí no se define GlobalLimiter
            options.AddPolicy("ipLimiter", httpContext =>
            {
                var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                return RateLimitPartition.GetFixedWindowLimiter(ip, _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 5,
                    Window = TimeSpan.FromSeconds(10),
                    QueueLimit = 0,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                });
            });
            
        });

        return services;
    }
    public static void AddJwt(this IServiceCollection services, IConfiguration configuration)
    {
        //Configuration from AppSettings
        services.Configure<JWT>(configuration.GetSection("JWT"));

        //Adding Athentication - JWT
        _ = services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = false;
                o.SaveToken = false;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = configuration["JWT:Issuer"],
                    ValidAudience = configuration["JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]!))
                };
            });
        // 3. Authorization – Policies
        services.AddAuthorization(options =>
        {
            // Política que exige rol Admin
            options.AddPolicy("Admins", policy =>
                policy.RequireRole("Administrator"));

            options.AddPolicy("Others", policy =>
                policy.RequireRole("Other"));

            options.AddPolicy("Pro", policy =>
                policy.RequireRole("Professional"));

            // Política que exige claim Subscription = "Premium"
            options.AddPolicy("Professional", policy =>
                policy.RequireClaim("Subscription", "Premium"));

            // Política compuesta: rol Admin o claim Premium
            options.AddPolicy("OtherOPremium", policy =>
                policy.RequireAssertion(context =>
                    context.User.IsInRole("Other")
                || context.User.HasClaim(c =>
                        c.Type == "Subscription" && c.Value == "Premium")));
        });
    }
    public static void AddValidationErrors(this IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = actionContext =>
            {

                var errors = actionContext.ModelState.Where(u => u.Value!.Errors.Count > 0)
                                                .SelectMany(u => u.Value!.Errors)
                                                .Select(u => u.ErrorMessage).ToArray();

                var errorResponse = new ApiValidation()
                {
                    Errors = errors
                };

                return new BadRequestObjectResult(errorResponse);
            };
        });
    }
}
