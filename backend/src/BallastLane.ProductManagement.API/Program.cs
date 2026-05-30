using System.Reflection;
using System.Text;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NHibernate;
using Serilog;
using BallastLane.ProductManagement.API.Middleware;
using BallastLane.ProductManagement.Domain.Interfaces;
using BallastLane.ProductManagement.Application.Interfaces;
using BallastLane.ProductManagement.Application.Services;
using BallastLane.ProductManagement.Infrastructure.Mappings;
using BallastLane.ProductManagement.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Serilog
builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration);
});

var configuration = builder.Configuration;
var services = builder.Services;

// NHibernate
var connectionString = configuration.GetConnectionString("DefaultConnection")!;
var sessionFactory = Fluently.Configure()
    .Database(MsSqlConfiguration.MsSql2012.ConnectionString(connectionString))
    .Mappings(m => m.FluentMappings.AddFromAssembly(Assembly.GetAssembly(typeof(ProductMap))))
    .BuildSessionFactory();

services.AddSingleton<ISessionFactory>(sessionFactory);

// Repositories (Infrastructure)
services.AddScoped<IProductRepository, ProductRepository>();
services.AddScoped<IUserRepository, UserRepository>();

// Services (Application)
services.AddScoped<IProductService, ProductService>();
services.AddScoped<IAuthService, AuthService>();

// JWT Authentication
var jwtSection = configuration.GetSection("Jwt");
var secret = jwtSection["Secret"]!;

services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
        };
    });

services.AddAuthorization();

services.AddCors(c => c.AddPolicy("AllowOrigin", options =>
{
    options.WithOrigins("http://localhost:4200", "http://localhost")
           .WithMethods("GET", "POST", "PUT", "DELETE")
           .WithHeaders("Content-Type", "Authorization");
}));

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "BallastLane Product Management API",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseCors("AllowOrigin");

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "V1");
});

app.UseSerilogRequestLogging();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Logger.LogInformation("BallastLane.ProductManagement.API - Starting up.");

app.Run();

public partial class Program { }
