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
        if (!string.IsNullOrWhiteSpace(extra)) origins.Add(extra);

        policy.WithOrigins(origins.ToArray())
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// ─── Database (SQLite) ────────────────────────────────────────────────────────
// Resolve the connection string with a safe fallback chain:
//   1. ConnectionStrings__DefaultConnection env var  (Render / Docker)
//   2. ConnectionStrings:DefaultConnection in appsettings.json
//   3. Hard-coded fallback so the app never crashes on a missing/empty value
var rawCs = builder.Configuration.GetConnectionString("DefaultConnection");

// Guard: if the value is null, empty, or doesn't look like an SQLite DSN,
// build a safe default pointing at the persistent-disk path on Render,
// or the current directory when running locally.
string connectionString;
if (string.IsNullOrWhiteSpace(rawCs) || !rawCs.Contains('='))
{
    // On Render the disk is mounted at /data; locally write next to the binary.
    var dbDir  = Directory.Exists("/data") ? "/data" : Directory.GetCurrentDirectory();
    connectionString = $"Data Source={Path.Combine(dbDir, "rentacar.db")}";
    Console.WriteLine($"[Startup] ConnectionString was empty — using fallback: {connectionString}");
}
else
{
    connectionString = rawCs;
    Console.WriteLine($"[Startup] Using connection string: {connectionString}");
}

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite(connectionString));

// ─── JWT Authentication ───────────────────────────────────────────────────────
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Jwt:Key must be set (env var Jwt__Key or appsettings.json).");

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

// ─── Auto-create DB + uploads folder ─────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();

    // Ensure the uploads folder exists (volume-mounted on Render/Docker)
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