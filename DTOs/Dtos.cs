namespace RentACar.API.DTOs;

// ──────────────────────────── Generic responses ───────────────────────────────

public class ResponseModel
{
    public bool   Success { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class SingleResponseModel<T> : ResponseModel
{
    public T? Data { get; set; }
}

public class ListResponseModel<T> : ResponseModel
{
    public List<T> Data { get; set; } = new();
}

// ──────────────────────────── Auth ────────────────────────────────────────────

public class LoginRequest
{
    public string Email    { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class RegisterRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName  { get; set; } = string.Empty;
    public string Email     { get; set; } = string.Empty;
    public string Password  { get; set; } = string.Empty;
}

public class TokenModel
{
    public string Token      { get; set; } = string.Empty;
    public string Expiration { get; set; } = string.Empty;
}

public class ChangePasswordRequest
{
    public int    UserId      { get; set; }
    public string OldPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}

// ──────────────────────────── Cars ────────────────────────────────────────────

/// <summary>Flat DTO returned by GET /cars/getcardetails (list view + home page)</summary>
public class CarDetailDto
{
    public int     CarId       { get; set; }
    public int     BrandId     { get; set; }
    public int     ColorId     { get; set; }
    public string  CarName     { get; set; } = string.Empty;
    public string  BrandName   { get; set; } = string.Empty;
    public string  ColorName   { get; set; } = string.Empty;
    public string  ModelYear   { get; set; } = string.Empty;
    public decimal DailyPrice  { get; set; }
    public string  Description { get; set; } = string.Empty;
    public string  ImagePath   { get; set; } = "/uploads/default.jpg";
}

/// <summary>Payload for adding or updating a car (admin)</summary>
public class CarUpsertRequest
{
    public int?    CarId       { get; set; }
    public int     BrandId     { get; set; }
    public int     ColorId     { get; set; }
    public string  CarName     { get; set; } = string.Empty;
    public string  ModelYear   { get; set; } = string.Empty;
    public decimal DailyPrice  { get; set; }
    public string  Description { get; set; } = string.Empty;
}

// ──────────────────────────── Brands ──────────────────────────────────────────

public class BrandDto
{
    public int    BrandId   { get; set; }
    public string BrandName { get; set; } = string.Empty;
}

// ──────────────────────────── Colors ──────────────────────────────────────────

public class ColorDto
{
    public int    ColorId   { get; set; }
    public string ColorName { get; set; } = string.Empty;
}

// ──────────────────────────── Rentals ─────────────────────────────────────────

public class RentalRequest
{
    public int      CarId          { get; set; }
    public DateTime RentDate       { get; set; }
    public DateTime ReturnDate     { get; set; }
    public decimal  TotalRentPrice { get; set; }
}

public class RentalDto
{
    public int      RentalId       { get; set; }
    public int      CarId          { get; set; }
    public int      UserId         { get; set; }
    public DateTime RentDate       { get; set; }
    public DateTime ReturnDate     { get; set; }
    public decimal  TotalRentPrice { get; set; }
}

// ──────────────────────────── Users ───────────────────────────────────────────

public class UserDto
{
    public int    UserId    { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName  { get; set; } = string.Empty;
    public string Email     { get; set; } = string.Empty;
}

// ──────────────────────────── Credit Cards ────────────────────────────────────

public class CreditCardDto
{
    public int     Id             { get; set; }
    public string  CardName       { get; set; } = string.Empty;
    public string  CardNumber     { get; set; } = string.Empty;
    public string  CardCvc        { get; set; } = string.Empty;
    public string  CardExpiration { get; set; } = string.Empty;
    public decimal MoneyInTheCard { get; set; }
}

public class CreditCardCheckRequest
{
    public string CardName       { get; set; } = string.Empty;
    public string CardNumber     { get; set; } = string.Empty;
    public string CardCvc        { get; set; } = string.Empty;
    public string CardExpiration { get; set; } = string.Empty;
}
