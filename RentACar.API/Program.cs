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

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "RentACar API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header. Enter 'Bearer' [space] and your token",
        Name        = "Authorization",
        In          = ParameterLocation.Header,
        Type        = SecuritySchemeType.ApiKey,
        Scheme      = "Bearer"
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

// ── Connection string ─────────────────────────────────────────────────────────
static string ConvertPostgresUrl(string cs)
{
    if (!cs.StartsWith("postgres://") && !cs.StartsWith("postgresql://"))
        return cs;
    var uri      = new Uri(cs);
    var userInfo = uri.UserInfo.Split(':', 2);
    var user     = userInfo[0];
    var pass     = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : "";
    var db       = uri.AbsolutePath.TrimStart('/');
    return $"Host={uri.Host};Port={uri.Port};Database={db};Username={user};Password={pass};SSL Mode=Require;Trust Server Certificate=true";
}

var rawConn = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
              ?? builder.Configuration.GetConnectionString("DefaultConnection")
              ?? "";

if (string.IsNullOrWhiteSpace(rawConn) || rawConn == "NOT_SET_USE_ENV_VAR")
    throw new InvalidOperationException(
        "Connection string not found. Set ConnectionStrings__DefaultConnection env var.");

var connectionString = ConvertPostgresUrl(rawConn);

builder.Services.AddDbContext<RentACarContext>(o => o.UseNpgsql(connectionString));

// ── Business services ─────────────────────────────────────────────────────────
builder.Services.AddScoped<IAuthService,   AuthManager>();
builder.Services.AddScoped<IUserService,   UserManager>();
builder.Services.AddScoped<IBrandService,  BrandManager>();
builder.Services.AddScoped<ICarService,    CarManager>();
builder.Services.AddScoped<IColorService,  ColorManager>();
builder.Services.AddScoped<IRentalService, RentalManager>();

builder.Services.AddScoped<IUserDal,   EfUserDal>();
builder.Services.AddScoped<IBrandDal,  EfBrandDal>();
builder.Services.AddScoped<ICarDal,    EfCarDal>();
builder.Services.AddScoped<IColorDal,  EfColorDal>();
builder.Services.AddScoped<IRentalDal, EfRentalDal>();

builder.Services.AddScoped<ITokenHelper, JwtHelper>();

// ── JWT ───────────────────────────────────────────────────────────────────────
var jwtAudience = Environment.GetEnvironmentVariable("TokenOptions__Audience")    ?? "www.rentacar.com";
var jwtIssuer   = Environment.GetEnvironmentVariable("TokenOptions__Issuer")      ?? "www.rentacar.com";
var jwtKey      = Environment.GetEnvironmentVariable("TokenOptions__SecurityKey")  ?? "GadYTiN03RWcjEoISKhwyMBFbQx9nD5ukp2J8mLq";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer              = jwtIssuer,
            ValidAudience            = jwtAudience,
            IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew                = TimeSpan.Zero
        };
    });

// ── CORS ──────────────────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", p => p
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());
});

builder.Services.AddHealthChecks()
    .AddNpgSql(connectionString, name: "database");

// ─────────────────────────────────────────────────────────────────────────────
var app = builder.Build();

// Swagger available in ALL environments for easier debugging
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "RentACar API V1");
    c.RoutePrefix = "swagger";
});

app.UseMiddleware<ExceptionMiddleware>();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

// ── DB init ───────────────────────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<RentACarContext>();
        logger.LogInformation("Testing DB connection…");

        var canConnect = await db.Database.CanConnectAsync();
        logger.LogInformation("DB CanConnect = {v}", canConnect);

        if (!canConnect)
            throw new Exception("Cannot reach the database. Verify the connection string.");

        var created = await db.Database.EnsureCreatedAsync();
        logger.LogInformation(created ? "Schema created." : "Schema already exists.");
    }
    catch (Exception ex)
    {
        var l = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        l.LogError(ex, "DB init failed: {Msg}", ex.Message);
    }
}

app.Run();