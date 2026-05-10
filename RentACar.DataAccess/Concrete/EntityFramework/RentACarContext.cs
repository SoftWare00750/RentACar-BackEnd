using Microsoft.EntityFrameworkCore;
using RentACar.Entities.Concrete;

namespace RentACar.DataAccess.Concrete.EntityFramework
{
    public class RentACarContext : DbContext
    {
        // Parameterless constructor required by EfEntityRepositoryBase
        public RentACarContext()
        {
        }

        // DI constructor
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