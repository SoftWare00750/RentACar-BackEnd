using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RentACar.API.Middleware;
using RentACar.Business.Abstract;
using RentACar.Business.Concrete;
using RentACar.Core.Utilities.Security.JWT;
using RentACar.DataAccess.Abstract;
using RentACar.DataAccess.Concrete.EntityFramework;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "RentACar API",
        Version = "v1",
        Description = "Car Rental Management System API"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// Build connection string
var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString) || connectionString == "NOT_SET_USE_ENV_VAR")
    throw new InvalidOperationException(
        "Connection string not found. Set the ConnectionStrings__DefaultConnection environment variable.");

// Render provides postgres:// URLs; convert to Npgsql key-value format
if (connectionString.StartsWith("postgres://") || connectionString.StartsWith("postgresql://"))
{
    var uri = new Uri(connectionString);
    var userInfo = uri.UserInfo.Split(':');
    connectionString = $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};Username={userInfo[0]};Password={Uri.UnescapeDataString(userInfo[1])};SSL Mode=Require;Trust Server Certificate=true";
}

builder.Services.AddDbContext<RentACarContext>(options =>
    options.UseNpgsql(connectionString));

// Dependency Injection - Services
builder.Services.AddScoped<IAuthService, AuthManager>();
builder.Services.AddScoped<IUserService, UserManager>();
builder.Services.AddScoped<IBrandService, BrandManager>();
builder.Services.AddScoped<ICarService, CarManager>();
builder.Services.AddScoped<IColorService, ColorManager>();
builder.Services.AddScoped<IRentalService, RentalManager>();

// Dependency Injection - Data Access
builder.Services.AddScoped<IUserDal, EfUserDal>();
builder.Services.AddScoped<IBrandDal, EfBrandDal>();
builder.Services.AddScoped<ICarDal, EfCarDal>();
builder.Services.AddScoped<IColorDal, EfColorDal>();
builder.Services.AddScoped<IRentalDal, EfRentalDal>();

// JWT Token Helper
builder.Services.AddScoped<ITokenHelper, JwtHelper>();

// Configure JWT Authentication
var tokenOptions = builder.Configuration.GetSection("TokenOptions").Get<TokenOptions>()
    ?? throw new InvalidOperationException("TokenOptions configuration not found");

// Allow token options to be overridden by environment variables
var audience = Environment.GetEnvironmentVariable("TokenOptions__Audience") ?? tokenOptions.Audience;
var issuer = Environment.GetEnvironmentVariable("TokenOptions__Issuer") ?? tokenOptions.Issuer;
var securityKey = Environment.GetEnvironmentVariable("TokenOptions__SecurityKey") ?? tokenOptions.SecurityKey;
var expiration = int.TryParse(Environment.GetEnvironmentVariable("TokenOptions__AccessTokenExpiration"), out var exp)
    ? exp
    : tokenOptions.AccessTokenExpiration;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policyBuilder =>
    {
        policyBuilder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });

    options.AddPolicy("Production", policyBuilder =>
    {
        var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>()
            ?? new[] { "https://renta-car-six.vercel.app" };

        policyBuilder
            .WithOrigins(allowedOrigins)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// Add Health Checks
builder.Services.AddHealthChecks()
    .AddNpgSql(connectionString, name: "database");

var app = builder.Build();

// Configure the HTTP request pipeline
// Show Swagger in all environments so you can debug on production
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "RentACar API V1");
    c.RoutePrefix = "swagger";
});

// Use exception middleware
app.UseMiddleware<ExceptionMiddleware>();

// CORS must come before Authentication/Authorization
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

// Initialize database — create tables if they don't exist
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<RentACarContext>();

        // Test connection first
        logger.LogInformation("Testing database connection...");
        await db.Database.CanConnectAsync();
        logger.LogInformation("Database connection successful.");

        // EnsureCreated creates tables only if the database is empty / tables missing
        var created = await db.Database.EnsureCreatedAsync();
        if (created)
            logger.LogInformation("Database tables created successfully.");
        else
            logger.LogInformation("Database already exists. Tables were not recreated.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while initializing the database: {Message}", ex.Message);
        // Don't throw — let the app start so /health and /swagger are reachable for debugging
    }
}

app.Run();