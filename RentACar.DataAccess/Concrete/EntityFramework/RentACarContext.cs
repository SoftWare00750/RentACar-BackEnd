using Microsoft.EntityFrameworkCore;
using RentACar.Entities.Concrete;

namespace RentACar.DataAccess.Concrete.EntityFramework
{
    public class RentACarContext : DbContext
    {
        // Parameterless ctor — only used when EfEntityRepositoryBase falls back to new TContext()
        public RentACarContext() { }

        // DI ctor — used in production via AddDbContext
        public RentACarContext(DbContextOptions<RentACarContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured) return;   // already set by DI — do nothing

            // Fallback: read env var directly (used only by parameterless ctor path)
            var raw = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                      ?? throw new InvalidOperationException(
                             "ConnectionStrings__DefaultConnection is not set.");

            optionsBuilder.UseNpgsql(ConvertPostgresUrl(raw));
        }

        private static string ConvertPostgresUrl(string cs)
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Car>(e =>
            {
                e.ToTable("Cars");
                e.HasKey(x => x.CarId);
            });

            modelBuilder.Entity<Brand>(e =>
            {
                e.ToTable("Brands");
                e.HasKey(x => x.BrandId);
            });

            modelBuilder.Entity<Color>(e =>
            {
                e.ToTable("Colors");
                e.HasKey(x => x.ColorId);
            });

            modelBuilder.Entity<Rental>(e =>
            {
                e.ToTable("Rentals");
                e.HasKey(x => x.RentalId);
            });

            modelBuilder.Entity<User>(e =>
            {
                e.ToTable("Users");
                e.HasKey(x => x.UserId);
            });

            modelBuilder.Entity<OperationClaim>(e =>
            {
                e.ToTable("OperationClaims");
                e.HasKey(x => x.Id);
            });

            modelBuilder.Entity<UserOperationClaim>(e =>
            {
                e.ToTable("UserOperationClaims");
                e.HasKey(x => x.Id);
            });
        }

        public DbSet<Car>               Cars                { get; set; }
        public DbSet<Brand>             Brands              { get; set; }
        public DbSet<Color>             Colors              { get; set; }
        public DbSet<Rental>            Rentals             { get; set; }
        public DbSet<User>              Users               { get; set; }
        public DbSet<OperationClaim>    OperationClaims     { get; set; }
        public DbSet<UserOperationClaim> UserOperationClaims { get; set; }
    }
}