using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentACar.API.Data;
using RentACar.API.DTOs;
using RentACar.API.Models;

namespace RentACar.API.Controllers;

[ApiController]
[Route("api/carImages")]
public class CarImagesController : ControllerBase
{
    private readonly AppDbContext  _db;
    private readonly IWebHostEnvironment _env;

    public CarImagesController(AppDbContext db, IWebHostEnvironment env)
    {
        _db  = db;
        _env = env;
    }

    /// <summary>All images for a car (public)</summary>
    [HttpGet("getimagesbycarid")]
    public async Task<IActionResult> GetImagesByCarId([FromQuery] int carId)
    {
        var data = await _db.CarImages
            .Where(ci => ci.CarId == carId)
            .OrderBy(ci => ci.ImageId)
            .ToListAsync();

        // Return default image if no images exist
        if (data.Count == 0)
            data.Add(new CarImage { ImageId = 0, CarId = carId, ImagePath = "/uploads/default.jpg" });

        return Ok(new ListResponseModel<CarImage> { Success = true, Message = "Images listed.", Data = data });
    }

    /// <summary>Upload an image for a car (admin only)</summary>
    [Authorize(Roles = "admin")]
    [HttpPost("upload/{carId:int}")]
    public async Task<IActionResult> Upload(int carId, IFormFile file)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new ResponseModel { Success = false, Message = "No file uploaded." });

        // Save to wwwroot/uploads/
        var uploadsDir = Path.Combine(_env.WebRootPath, "uploads");
        Directory.CreateDirectory(uploadsDir);

        var ext      = Path.GetExtension(file.FileName);
        var fileName = $"car-{carId}-{Guid.NewGuid():N}{ext}";
        var filePath = Path.Combine(uploadsDir, fileName);

        await using (var stream = System.IO.File.Create(filePath))
            await file.CopyToAsync(stream);

        var image = new CarImage { CarId = carId, ImagePath = $"/uploads/{fileName}" };
        _db.CarImages.Add(image);
        await _db.SaveChangesAsync();

        return Ok(new SingleResponseModel<CarImage>
        {
            Success = true, Message = "Image uploaded.", Data = image
        });
    }

    /// <summary>Delete an image (admin only)</summary>
    [Authorize(Roles = "admin")]
    [HttpPost("delete")]
    public async Task<IActionResult> Delete([FromBody] CarImage req)
    {
        var image = await _db.CarImages.FindAsync(req.ImageId);
        if (image is null) return NotFound(new ResponseModel { Success = false, Message = "Image not found." });

        // Remove physical file if it exists
        if (!image.ImagePath.Contains("default"))
        {
            var physical = Path.Combine(_env.WebRootPath, image.ImagePath.TrimStart('/'));
            if (System.IO.File.Exists(physical))
                System.IO.File.Delete(physical);
        }

        _db.CarImages.Remove(image);
        await _db.SaveChangesAsync();
        return Ok(new ResponseModel { Success = true, Message = "Image deleted." });
    }
}
