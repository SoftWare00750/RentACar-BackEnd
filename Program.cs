using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RentACar.API.Data;
using RentACar.API.Services;

var builder = WebApplication.CreateBuilder(args);

// ─── CORS ─────────────────────────────────────────────────────────────────────
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("AllowAngular", policy =>
    {
        var origins = new List<string>
        {
            "http://localhost:4200",
            "https://localhost:4200"
        };
        var extra = builder.Configuration["AllowedOrigins"];
        if (!string.IsNullOrWhiteSpace(extra))
        {
            // Support comma-separated list of origins
            foreach (var origin in extra.Split(',', StringSplitOptions.RemoveEmptyEntries))
                origins.Add(origin.Trim().TrimEnd('/'));
        }

        policy.WithOrigins(origins.ToArray())
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// ─── Database (PostgreSQL) ────────────────────────────────────────────────────
// Priority: env var DATABASE_URL (Render Postgres) → ConnectionStrings:DefaultConnection
// Render injects DATABASE_URL as a postgres:// URI; Npgsql needs it converted.
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
string connectionString;

if (!string.IsNullOrWhiteSpace(databaseUrl))
{
    // Convert postgres://user:pass@host:port/db  →  Npgsql DSN
    connectionString = ConvertPostgresUrl(databaseUrl);
    Console.WriteLine("[Startup] Using DATABASE_URL env var (PostgreSQL)");
}
else
{
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("No database connection string found. Set DATABASE_URL or ConnectionStrings:DefaultConnection.");
    Console.WriteLine($"[Startup] Using connection string from config: {MaskPassword(connectionString)}");
}

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(connectionString));

// ─── JWT Authentication ───────────────────────────────────────────────────────
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Jwt:Key must be set.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer              = builder.Configuration["Jwt:Issuer"] ?? "RentACar",
            ValidAudience            = builder.Configuration["Jwt:Issuer"] ?? "RentACar",
            IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew                = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddControllers();

var app = builder.Build();

// ─── Auto-create DB schema + uploads folder ───────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    // EnsureCreated works for both Postgres and SQLite
    db.Database.EnsureCreated();

    var webRoot = app.Environment.WebRootPath
                  ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
    Directory.CreateDirectory(Path.Combine(webRoot, "uploads"));
}

// ─── Pipeline ─────────────────────────────────────────────────────────────────
app.UseCors("AllowAngular");
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "healthy", time = DateTime.UtcNow }));

app.Run();

// ─── Helpers ──────────────────────────────────────────────────────────────────

// Converts a Render/Heroku-style postgres:// URI to an Npgsql connection string.
// postgres://user:password@host:port/database
static string ConvertPostgresUrl(string url)
{
    var uri      = new Uri(url);
    var userInfo = uri.UserInfo.Split(':');
    var user     = Uri.UnescapeDataString(userInfo[0]);
    var password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : "";
    var host     = uri.Host;
    var port     = uri.Port > 0 ? uri.Port : 5432;
    var database = uri.AbsolutePath.TrimStart('/');
    return $"Host={host};Port={port};Database={database};Username={user};Password={password};SSL Mode=Require;Trust Server Certificate=true";
}

static string MaskPassword(string cs)
{
    // Mask Password= value in logs
    return System.Text.RegularExpressions.Regex.Replace(
        cs, @"Password=[^;]+", "Password=***");
}