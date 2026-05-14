using Microsoft.EntityFrameworkCore;
using RentACar.API.Models;

namespace RentACar.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User>       Users       => Set<User>();
    public DbSet<Brand>      Brands      => Set<Brand>();
    public DbSet<Color>      Colors      => Set<Color>();
    public DbSet<Car>        Cars        => Set<Car>();
    public DbSet<CarImage>   CarImages   => Set<CarImage>();
    public DbSet<Rental>     Rentals     => Set<Rental>();
    public DbSet<CreditCard> CreditCards => Set<CreditCard>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        // ── Users ──────────────────────────────────────────────────────────────
        mb.Entity<User>(e =>
        {
            e.HasKey(u => u.UserId);
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.Email).HasMaxLength(200);
            e.Property(u => u.Role).HasDefaultValue("user");
        });

        // ── Brands ─────────────────────────────────────────────────────────────
        mb.Entity<Brand>(e =>
        {
            e.HasKey(b => b.BrandId);
            e.Property(b => b.BrandName).HasMaxLength(100).IsRequired();
        });

        // ── Colors ─────────────────────────────────────────────────────────────
        mb.Entity<Color>(e =>
        {
            e.HasKey(c => c.ColorId);
            e.Property(c => c.ColorName).HasMaxLength(50).IsRequired();
        });

        // ── Cars ───────────────────────────────────────────────────────────────
        mb.Entity<Car>(e =>
        {
            e.HasKey(c => c.CarId);
            e.Property(c => c.DailyPrice).HasColumnType("decimal(10,2)");
            e.HasOne(c => c.Brand)
             .WithMany(b => b.Cars)
             .HasForeignKey(c => c.BrandId)
             .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(c => c.Color)
             .WithMany(co => co.Cars)
             .HasForeignKey(c => c.ColorId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // ── CarImages ──────────────────────────────────────────────────────────
        mb.Entity<CarImage>(e =>
        {
            e.HasKey(ci => ci.ImageId);
            e.HasOne(ci => ci.Car)
             .WithMany(c => c.CarImages)
             .HasForeignKey(ci => ci.CarId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // ── Rentals ────────────────────────────────────────────────────────────
        mb.Entity<Rental>(e =>
        {
            e.HasKey(r => r.RentalId);
            e.Property(r => r.TotalRentPrice).HasColumnType("decimal(10,2)");
            e.HasOne(r => r.Car)
             .WithMany(c => c.Rentals)
             .HasForeignKey(r => r.CarId)
             .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(r => r.User)
             .WithMany(u => u.Rentals)
             .HasForeignKey(r => r.UserId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // ── CreditCards ────────────────────────────────────────────────────────
        mb.Entity<CreditCard>(e =>
        {
            e.HasKey(cc => cc.Id);
            e.Property(cc => cc.MoneyInTheCard).HasColumnType("decimal(12,2)");
        });

        // ── Seed data ──────────────────────────────────────────────────────────
        SeedData(mb);
    }

    private static void SeedData(ModelBuilder mb)
    {
        // Brands
        mb.Entity<Brand>().HasData(
            new Brand { BrandId = 1, BrandName = "Toyota"      },
            new Brand { BrandId = 2, BrandName = "Honda"       },
            new Brand { BrandId = 3, BrandName = "BMW"         },
            new Brand { BrandId = 4, BrandName = "Mercedes"    },
            new Brand { BrandId = 5, BrandName = "Ford"        },
            new Brand { BrandId = 6, BrandName = "Volkswagen"  },
            new Brand { BrandId = 7, BrandName = "Hyundai"     },
            new Brand { BrandId = 8, BrandName = "Nissan"      }
        );

        // Colors
        mb.Entity<Models.Color>().HasData(
            new Models.Color { ColorId = 1, ColorName = "White"  },
            new Models.Color { ColorId = 2, ColorName = "Black"  },
            new Models.Color { ColorId = 3, ColorName = "Silver" },
            new Models.Color { ColorId = 4, ColorName = "Red"    },
            new Models.Color { ColorId = 5, ColorName = "Blue"   },
            new Models.Color { ColorId = 6, ColorName = "Grey"   }
        );

        // Cars
        mb.Entity<Car>().HasData(
            new Car { CarId=1,  BrandId=1, ColorId=1, CarName="Toyota Corolla",        ModelYear="2022", DailyPrice=55,  Description="Reliable sedan, great fuel economy."      },
            new Car { CarId=2,  BrandId=1, ColorId=2, CarName="Toyota Camry",          ModelYear="2023", DailyPrice=75,  Description="Comfortable mid-size sedan."               },
            new Car { CarId=3,  BrandId=2, ColorId=3, CarName="Honda Civic",           ModelYear="2022", DailyPrice=50,  Description="Sporty compact car, smooth ride."          },
            new Car { CarId=4,  BrandId=2, ColorId=5, CarName="Honda CR-V",            ModelYear="2023", DailyPrice=90,  Description="Versatile SUV with ample cargo space."     },
            new Car { CarId=5,  BrandId=3, ColorId=2, CarName="BMW 3 Series",          ModelYear="2023", DailyPrice=150, Description="Luxury performance sedan."                 },
            new Car { CarId=6,  BrandId=3, ColorId=1, CarName="BMW X5",               ModelYear="2023", DailyPrice=200, Description="Premium SUV with advanced tech."           },
            new Car { CarId=7,  BrandId=4, ColorId=2, CarName="Mercedes C-Class",      ModelYear="2022", DailyPrice=160, Description="Elegant luxury sedan."                    },
            new Car { CarId=8,  BrandId=4, ColorId=1, CarName="Mercedes GLE",         ModelYear="2023", DailyPrice=220, Description="Top-tier luxury SUV."                     },
            new Car { CarId=9,  BrandId=5, ColorId=4, CarName="Ford Mustang",          ModelYear="2022", DailyPrice=130, Description="Iconic American muscle car."               },
            new Car { CarId=10, BrandId=5, ColorId=1, CarName="Ford F-150",           ModelYear="2023", DailyPrice=110, Description="America's best-selling pickup truck."      },
            new Car { CarId=11, BrandId=6, ColorId=6, CarName="Volkswagen Golf",      ModelYear="2022", DailyPrice=65,  Description="Classic hatchback, versatile and fun."     },
            new Car { CarId=12, BrandId=7, ColorId=5, CarName="Hyundai Tucson",       ModelYear="2023", DailyPrice=85,  Description="Modern SUV with great warranty."           },
            new Car { CarId=13, BrandId=8, ColorId=3, CarName="Nissan Altima",        ModelYear="2022", DailyPrice=70,  Description="Smooth sedan with ProPilot assist."        },
            new Car { CarId=14, BrandId=8, ColorId=4, CarName="Nissan Kicks",         ModelYear="2023", DailyPrice=60,  Description="Urban crossover with distinctive style."   }
        );

        // CarImages — one default image per car (uses public placeholder)
        var images = Enumerable.Range(1, 14).Select(id => new CarImage
        {
            ImageId   = id,
            CarId     = id,
            ImagePath = $"/uploads/car-{id}.jpg",
            Date      = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        }).ToArray();
        mb.Entity<CarImage>().HasData(images);

        // Seed one admin user (password: Admin@123)
        mb.Entity<User>().HasData(new User
        {
            UserId       = 1,
            FirstName    = "Admin",
            LastName     = "User",
            Email        = "admin@rentacar.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            Role         = "admin",
            CreatedAt    = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });

        // Seed test credit cards (for the credit card payment page)
        mb.Entity<CreditCard>().HasData(
            new CreditCard { Id=1, CardName="Test User",    CardNumber="4000056655665556", CardCvc="123", CardExpiration="12/26", MoneyInTheCard=50000 },
            new CreditCard { Id=2, CardName="Test User",    CardNumber="5200828282828210", CardCvc="456", CardExpiration="06/27", MoneyInTheCard=30000 },
            new CreditCard { Id=3, CardName="Test Admin",   CardNumber="371449635398431",  CardCvc="789", CardExpiration="09/25", MoneyInTheCard=99999 },
            new CreditCard { Id=4, CardName="Demo Account", CardNumber="6011000990139424", CardCvc="321", CardExpiration="03/26", MoneyInTheCard=15000 }
        );
    }
}
