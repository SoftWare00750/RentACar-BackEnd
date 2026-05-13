using Microsoft.EntityFrameworkCore;
using RentACar.Entities.Concrete;

namespace RentACar.DataAccess.Concrete.EntityFramework
{
    public class RentACarContext : DbContext
    {
        // Parameterless constructor required by EfEntityRepositoryBase fallback
        public RentACarContext()
        {
        }

        // DI constructor — preferred in production
        public RentACarContext(DbContextOptions<RentACarContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString =
                    Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                    ?? throw new InvalidOperationException(
                        "ConnectionStrings__DefaultConnection environment variable is not set.");

                // Convert Render postgres:// URL format if needed
                if (connectionString.StartsWith("postgres://") || connectionString.StartsWith("postgresql://"))
                {
                    var uri = new Uri(connectionString);
                    var userInfo = uri.UserInfo.Split(':');
                    connectionString = $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};Username={userInfo[0]};Password={Uri.UnescapeDataString(userInfo[1])};SSL Mode=Require;Trust Server Certificate=true";
                }

                optionsBuilder.UseNpgsql(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Explicit table names to avoid EF pluralization surprises
            modelBuilder.Entity<Car>().ToTable("Cars");
            modelBuilder.Entity<Brand>().ToTable("Brands");
            modelBuilder.Entity<Color>().ToTable("Colors");
            modelBuilder.Entity<Rental>().ToTable("Rentals");
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<OperationClaim>().ToTable("OperationClaims");
            modelBuilder.Entity<UserOperationClaim>().ToTable("UserOperationClaims");

            // Primary keys (EF may not infer these correctly without conventions)
            modelBuilder.Entity<Car>().HasKey(c => c.CarId);
            modelBuilder.Entity<Brand>().HasKey(b => b.BrandId);
            modelBuilder.Entity<Color>().HasKey(c => c.ColorId);
            modelBuilder.Entity<Rental>().HasKey(r => r.RentalId);
            modelBuilder.Entity<User>().HasKey(u => u.UserId);
            modelBuilder.Entity<OperationClaim>().HasKey(o => o.Id);
            modelBuilder.Entity<UserOperationClaim>().HasKey(u => u.Id);
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