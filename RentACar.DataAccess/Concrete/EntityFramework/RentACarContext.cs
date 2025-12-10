using Microsoft.EntityFrameworkCore;
using RentACar.Entities.Concrete;

namespace RentACar.DataAccess.Concrete.EntityFramework
{
    public class RentACarContext : DbContext
    {
        // REQUIRED by EfEntityRepositoryBase
        public RentACarContext()
        {
        }

        // OPTIONAL for DI (Program.cs)
        public RentACarContext(DbContextOptions<RentACarContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    if (!optionsBuilder.IsConfigured)
    {
        // This will use the connection string from appsettings.json or environment variable
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection") 
            ?? "Host=dpg-d4s8l2ndiees73a8ko8g-a;Port=5432;Database=rentacardb;Username=rentacar_user;Password=AOdeXyXXkOfcKhC2nqjzoRRwtljGUAnB";
        
        optionsBuilder.UseNpgsql(connectionString);
    }
}

        public DbSet<Car> Cars { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<Rental> Rentals { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<OperationClaim> OperationClaims { get; set; }
        public DbSet<UserOperationClaim> UserOperationClaims { get; set; }
    }
}
