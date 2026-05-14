using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentACar.API.Data;
using RentACar.API.DTOs;
using RentACar.API.Models;

namespace RentACar.API.Controllers;

[ApiController]
[Route("api/rentals")]
public class RentalsController : ControllerBase
{
    private readonly AppDbContext _db;
    public RentalsController(AppDbContext db) => _db = db;

    /// <summary>All rentals with detail (admin only)</summary>
    [Authorize(Roles = "admin")]
    [HttpGet("getallrentaldto")]
    public async Task<IActionResult> GetAllRentals()
    {
        var data = await _db.Rentals
            .Include(r => r.Car).ThenInclude(c => c!.Brand)
            .Include(r => r.User)
            .Select(r => new RentalDto
            {
                RentalId       = r.RentalId,
                CarId          = r.CarId,
                UserId         = r.UserId,
                RentDate       = r.RentDate,
                ReturnDate     = r.ReturnDate,
                TotalRentPrice = r.TotalRentPrice
            })
            .ToListAsync();

        return Ok(new ListResponseModel<RentalDto> { Success = true, Message = "Rentals listed.", Data = data });
    }

    /// <summary>Check whether a car is rentable for given dates</summary>
    [Authorize]
    [HttpPost("isrentable")]
    public async Task<IActionResult> IsRentable([FromBody] RentalRequest req)
    {
        var overlap = await _db.Rentals.AnyAsync(r =>
            r.CarId == req.CarId &&
            r.RentDate   < req.ReturnDate &&
            r.ReturnDate > req.RentDate);

        return Ok(new ResponseModel
        {
            Success = !overlap,
            Message = overlap ? "Car is not available for the selected dates." : "Car is available."
        });
    }

    /// <summary>Create a rental (authenticated users)</summary>
    [Authorize]
    [HttpPost("add")]
    public async Task<IActionResult> AddRental([FromBody] RentalRequest req)
    {
        // Extract userId from JWT
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out var userId))
            return Unauthorized(new ResponseModel { Success = false, Message = "Cannot identify user." });

        // Double-check availability
        var overlap = await _db.Rentals.AnyAsync(r =>
            r.CarId == req.CarId &&
            r.RentDate   < req.ReturnDate &&
            r.ReturnDate > req.RentDate);

        if (overlap)
            return Conflict(new ResponseModel { Success = false, Message = "Car already rented for those dates." });

        var rental = new Rental
        {
            CarId          = req.CarId,
            UserId         = userId,
            RentDate       = req.RentDate,
            ReturnDate     = req.ReturnDate,
            TotalRentPrice = req.TotalRentPrice
        };

        _db.Rentals.Add(rental);
        await _db.SaveChangesAsync();

        return Ok(new SingleResponseModel<RentalDto>
        {
            Success = true,
            Message = "Rental created successfully.",
            Data    = new RentalDto
            {
                RentalId       = rental.RentalId,
                CarId          = rental.CarId,
                UserId         = rental.UserId,
                RentDate       = rental.RentDate,
                ReturnDate     = rental.ReturnDate,
                TotalRentPrice = rental.TotalRentPrice
            }
        });
    }
}
