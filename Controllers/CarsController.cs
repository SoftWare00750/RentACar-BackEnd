using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentACar.API.Data;
using RentACar.API.DTOs;
using RentACar.API.Models;

namespace RentACar.API.Controllers;

[ApiController]
[Route("api/cars")]
public class CarsController : ControllerBase
{
    private readonly AppDbContext _db;
    public CarsController(AppDbContext db) => _db = db;

    // ── Helpers ───────────────────────────────────────────────────────────────

    private IQueryable<CarDetailDto> CarDetailQuery() =>
        _db.Cars
           .Include(c => c.Brand)
           .Include(c => c.Color)
           .Include(c => c.CarImages)
           .Select(c => new CarDetailDto
           {
               CarId       = c.CarId,
               BrandId     = c.BrandId,
               ColorId     = c.ColorId,
               CarName     = c.CarName,
               BrandName   = c.Brand!.BrandName,
               ColorName   = c.Color!.ColorName,
               ModelYear   = c.ModelYear,
               DailyPrice  = c.DailyPrice,
               Description = c.Description,
               ImagePath   = c.CarImages.OrderBy(i => i.ImageId)
                               .Select(i => i.ImagePath)
                               .FirstOrDefault() ?? "/uploads/default.jpg"
           });

    // ── GET endpoints (public) ────────────────────────────────────────────────

    /// <summary>All cars with details — used by home page and cars list</summary>
    [HttpGet("getcardetails")]
    public async Task<IActionResult> GetCarDetails()
    {
        var data = await CarDetailQuery().ToListAsync();
        return Ok(new ListResponseModel<CarDetailDto>
        {
            Success = true, Message = "Cars listed.", Data = data
        });
    }

    /// <summary>Single car detail (returns list with one item — matches Angular usage)</summary>
    [HttpGet("getcardetail/{carId:int}")]
    public async Task<IActionResult> GetCarDetail(int carId)
    {
        var data = await CarDetailQuery().Where(c => c.CarId == carId).ToListAsync();
        return Ok(new ListResponseModel<CarDetailDto>
        {
            Success = true, Message = "Car detail retrieved.", Data = data
        });
    }

    /// <summary>Get single car by ID (for admin edit form)</summary>
    [HttpGet("getbyid")]
    public async Task<IActionResult> GetById([FromQuery] int carId)
    {
        var car = await _db.Cars.FindAsync(carId);
        if (car is null) return NotFound(new ResponseModel { Success = false, Message = "Car not found." });
        return Ok(new SingleResponseModel<Car> { Success = true, Message = "OK.", Data = car });
    }

    /// <summary>Filter by brand</summary>
    [HttpGet("getcarsbybrandid")]
    public async Task<IActionResult> GetCarsByBrandId([FromQuery] int brandId)
    {
        var data = await CarDetailQuery().Where(c => c.BrandId == brandId).ToListAsync();
        return Ok(new ListResponseModel<CarDetailDto> { Success = true, Message = "Filtered by brand.", Data = data });
    }

    /// <summary>Filter by color</summary>
    [HttpGet("getcarsbycolorid")]
    public async Task<IActionResult> GetCarsByColorId([FromQuery] int colorId)
    {
        var data = await CarDetailQuery().Where(c => c.ColorId == colorId).ToListAsync();
        return Ok(new ListResponseModel<CarDetailDto> { Success = true, Message = "Filtered by color.", Data = data });
    }

    /// <summary>Filter by brand AND color</summary>
    [HttpGet("getcarsbybrandandcolorid")]
    public async Task<IActionResult> GetCarsByBrandAndColorId([FromQuery] int brandId, [FromQuery] int colorId)
    {
        var data = await CarDetailQuery()
            .Where(c => c.BrandId == brandId && c.ColorId == colorId)
            .ToListAsync();
        return Ok(new ListResponseModel<CarDetailDto> { Success = true, Message = "Filtered by brand and color.", Data = data });
    }

    /// <summary>Full-text search by car name or description</summary>
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string q)
    {
        var lower = q.ToLower();
        var data  = await CarDetailQuery()
            .Where(c => c.CarName.ToLower().Contains(lower) || c.Description.ToLower().Contains(lower))
            .ToListAsync();
        return Ok(new ListResponseModel<CarDetailDto> { Success = true, Message = "Search results.", Data = data });
    }

    // ── Admin write operations ────────────────────────────────────────────────

    /// <summary>Add a new car (admin only)</summary>
    [Authorize(Roles = "admin")]
    [HttpPost("add")]
    public async Task<IActionResult> AddCar([FromBody] CarUpsertRequest req)
    {
        var car = new Car
        {
            BrandId     = req.BrandId,
            ColorId     = req.ColorId,
            CarName     = req.CarName,
            ModelYear   = req.ModelYear,
            DailyPrice  = req.DailyPrice,
            Description = req.Description
        };
        _db.Cars.Add(car);
        await _db.SaveChangesAsync();

        // Add a default image entry
        _db.CarImages.Add(new CarImage { CarId = car.CarId, ImagePath = "/uploads/default.jpg" });
        await _db.SaveChangesAsync();

        return Ok(new ResponseModel { Success = true, Message = "Car added successfully." });
    }

    /// <summary>Update a car (admin only)</summary>
    [Authorize(Roles = "admin")]
    [HttpPost("update")]
    public async Task<IActionResult> UpdateCar([FromBody] CarUpsertRequest req)
    {
        var car = await _db.Cars.FindAsync(req.CarId);
        if (car is null) return NotFound(new ResponseModel { Success = false, Message = "Car not found." });

        car.BrandId     = req.BrandId;
        car.ColorId     = req.ColorId;
        car.CarName     = req.CarName;
        car.ModelYear   = req.ModelYear;
        car.DailyPrice  = req.DailyPrice;
        car.Description = req.Description;

        await _db.SaveChangesAsync();
        return Ok(new ResponseModel { Success = true, Message = "Car updated successfully." });
    }

    /// <summary>Delete a car (admin only)</summary>
    [Authorize(Roles = "admin")]
    [HttpPost("delete")]
    public async Task<IActionResult> DeleteCar([FromBody] CarUpsertRequest req)
    {
        var car = await _db.Cars.FindAsync(req.CarId);
        if (car is null) return NotFound(new ResponseModel { Success = false, Message = "Car not found." });

        _db.Cars.Remove(car);
        await _db.SaveChangesAsync();
        return Ok(new ResponseModel { Success = true, Message = "Car deleted." });
    }
}
