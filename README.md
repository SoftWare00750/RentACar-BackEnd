# RentACar Backend API — .NET 8

A clean, production-ready ASP.NET Core 8 Web API backend for the RentACar Angular frontend.

---

## 🗂 Project Structure

```
RentACar.API/
├── Controllers/
│   ├── AuthController.cs         — login, register, change password
│   ├── CarsController.cs         — full CRUD + filter/search
│   ├── BrandsController.cs       — brand management
│   ├── ColorsController.cs       — color management
│   ├── CarImagesController.cs    — image upload/delete
│   ├── RentalsController.cs      — rental bookings + availability
│   ├── UsersController.cs        — user profile
│   ├── CreditCardsController.cs  — fake payment processing
│   └── CustomersController.cs    — stub (frontend compatibility)
├── Data/
│   └── AppDbContext.cs           — EF Core context + seed data
├── DTOs/
│   └── Dtos.cs                   — all request/response objects
├── Models/
│   ├── User.cs
│   ├── Car.cs
│   ├── Brand.cs
│   ├── Color.cs
│   ├── CarImage.cs
│   ├── Rental.cs
│   └── CreditCard.cs
├── Services/
│   └── TokenService.cs           — JWT token generation
├── wwwroot/uploads/              — car images served as static files
├── appsettings.json
└── Program.cs
```

---

## ⚡ Quick Start

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- No database installation needed — uses **SQLite** (file-based)

### 1. Restore packages & run

```bash
cd RentACar.API
dotnet restore
dotnet run
```

The API starts on **http://localhost:5000** (or https://localhost:5001).
The SQLite database (`rentacar.db`) and seed data are created automatically on first run.

### 2. Point the Angular frontend at this API

In `RentA-Car-FrontEnd/src/environments/environment.ts`:
```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000/api/'
};
```

### 3. Default admin account (seeded)

| Field    | Value              |
|----------|--------------------|
| Email    | admin@rentacar.com |
| Password | Admin@123          |
| Role     | admin              |

### 4. Test credit cards (seeded)

| Card Number          | CVC | Expiry | Balance  |
|----------------------|-----|--------|----------|
| 4000056655665556     | 123 | 12/26  | $50,000  |
| 5200828282828210     | 456 | 06/27  | $30,000  |
| 371449635398431      | 789 | 09/25  | $99,999  |
| 6011000990139424     | 321 | 03/26  | $15,000  |

---

## 🔧 Configuration (appsettings.json)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=rentacar.db"
  },
  "Jwt": {
    "Key": "CHANGE_THIS_TO_A_LONG_SECRET_KEY_IN_PRODUCTION",
    "Issuer": "RentACar",
    "ExpiresMinutes": "480"
  },
  "AllowedOrigins": "https://your-deployed-frontend.com"
}
```

> ⚠️ **Production**: Change the `Jwt:Key` to a long random secret (≥ 32 chars).
> Use environment variables or Azure Key Vault instead of appsettings for secrets.

---

## 📋 API Reference

All endpoints return JSON in this shape:
```json
{ "success": true, "message": "...", "data": { } }
```

### Auth (`/api/auth`)

| Method | Endpoint            | Auth     | Body                                 |
|--------|---------------------|----------|--------------------------------------|
| POST   | `/login`            | Public   | `{ email, password }`               |
| POST   | `/register`         | Public   | `{ firstName, lastName, email, password }` |
| POST   | `/changepassword`   | Bearer   | `{ userId, oldPassword, newPassword }` |

### Cars (`/api/cars`)

| Method | Endpoint                        | Auth       | Notes                     |
|--------|---------------------------------|------------|---------------------------|
| GET    | `/getcardetails`                | Public     | All cars (home + list)    |
| GET    | `/getcardetail/{carId}`         | Public     | Single car detail         |
| GET    | `/getbyid?carId=`               | Public     | Raw car (admin edit)      |
| GET    | `/getcarsbybrandid?brandId=`    | Public     | Filter by brand           |
| GET    | `/getcarsbycolorid?colorId=`    | Public     | Filter by color           |
| GET    | `/getcarsbybrandandcolorid?brandId=&colorId=` | Public | Filter both  |
| GET    | `/search?q=`                    | Public     | Full-text search          |
| POST   | `/add`                          | Admin only | Add car                   |
| POST   | `/update`                       | Admin only | Update car                |
| POST   | `/delete`                       | Admin only | Delete car                |

### Brands (`/api/brands`)

| Method | Endpoint             | Auth       |
|--------|----------------------|------------|
| GET    | `/getall`            | Public     |
| GET    | `/getbyid?brandId=`  | Public     |
| POST   | `/add`               | Admin only |
| POST   | `/updated`           | Admin only |
| POST   | `/delete`            | Admin only |

### Colors (`/api/colors`)

| Method | Endpoint             | Auth       |
|--------|----------------------|------------|
| GET    | `/getall`            | Public     |
| GET    | `/getbyid?colorId=`  | Public     |
| POST   | `/add`               | Admin only |
| POST   | `/updated`           | Admin only |
| POST   | `/delete`            | Admin only |

### Car Images (`/api/carImages`)

| Method | Endpoint                            | Auth       |
|--------|-------------------------------------|------------|
| GET    | `/getimagesbycarid?carId=`          | Public     |
| POST   | `/upload/{carId}` (multipart/form)  | Admin only |
| POST   | `/delete`                           | Admin only |

### Rentals (`/api/rentals`)

| Method | Endpoint              | Auth       |
|--------|-----------------------|------------|
| GET    | `/getallrentaldto`    | Admin only |
| POST   | `/isrentable`         | Bearer     |
| POST   | `/add`                | Bearer     |

### Users (`/api/users`)

| Method | Endpoint              | Auth   |
|--------|-----------------------|--------|
| GET    | `/getbyid?userId=`    | Bearer |
| PUT    | `/updated`            | Bearer |

### Credit Cards (`/api/creditcards`)

| Method | Endpoint              | Auth   |
|--------|-----------------------|--------|
| POST   | `/iscardexist`        | Bearer |
| GET    | `/getbycardnumber?cardnumber=` | Bearer |
| PUT    | `/update`             | Bearer |

---

## 🗄 Database

- **Engine**: SQLite (file: `rentacar.db` in the run directory)
- **ORM**: Entity Framework Core 8
- **Migration strategy**: `EnsureCreated()` on startup — no manual migrations needed

To reset the database, simply delete `rentacar.db` and restart the app.

### Schema

```
Users          — UserId, FirstName, LastName, Email, PasswordHash, Role
Brands         — BrandId, BrandName
Colors         — ColorId, ColorName
Cars           — CarId, BrandId, ColorId, CarName, ModelYear, DailyPrice, Description
CarImages      — ImageId, CarId, ImagePath, Date
Rentals        — RentalId, CarId, UserId, RentDate, ReturnDate, TotalRentPrice
CreditCards    — Id, CardName, CardNumber, CardCvc, CardExpiration, MoneyInTheCard
```

---

## 🔐 Authentication

JWT Bearer tokens. The Angular frontend stores the token in localStorage under `"token"`.

The token payload contains:
- `nameidentifier` — user ID
- `name` — full name (`"FirstName LastName"`)
- `email`
- `role` — `"user"` or `"admin"`

---

## 🚀 Deployment

### Docker (one-command)

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish RentACar.API.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "RentACar.API.dll"]
```

### Environment variables for production

```bash
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection="Data Source=/data/rentacar.db"
Jwt__Key="your-very-long-production-secret-key-here"
AllowedOrigins="https://your-frontend-domain.com"
```

---

## 🧰 Tech Stack

| Layer          | Technology                    |
|----------------|-------------------------------|
| Framework      | ASP.NET Core 8                |
| ORM            | Entity Framework Core 8       |
| Database       | SQLite (dev) / any EF provider|
| Auth           | JWT Bearer (HS512)            |
| Password hash  | BCrypt.Net-Next               |
| CORS           | Built-in ASP.NET Core CORS    |
| Static files   | Built-in (wwwroot/uploads)    |
