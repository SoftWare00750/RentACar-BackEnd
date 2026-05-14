using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentACar.API.Data;
using RentACar.API.DTOs;
using RentACar.API.Models;

namespace RentACar.API.Controllers;

[ApiController]
[Route("api/colors")]
public class ColorsController : ControllerBase
{
    private readonly AppDbContext _db;
    public ColorsController(AppDbContext db) => _db = db;

    [HttpGet("getall")]
    public async Task<IActionResult> GetAll()
    {
        var data = await _db.Colors
            .OrderBy(c => c.ColorName)
            .Select(c => new ColorDto { ColorId = c.ColorId, ColorName = c.ColorName })
            .ToListAsync();
        return Ok(new ListResponseModel<ColorDto> { Success = true, Message = "Colors listed.", Data = data });
    }

    [HttpGet("getbyid")]
    public async Task<IActionResult> GetById([FromQuery] int colorId)
    {
        var color = await _db.Colors.FindAsync(colorId);
        if (color is null) return NotFound(new ResponseModel { Success = false, Message = "Color not found." });
        return Ok(new SingleResponseModel<ColorDto>
        {
            Success = true, Message = "OK.",
            Data    = new ColorDto { ColorId = color.ColorId, ColorName = color.ColorName }
        });
    }

    [Authorize(Roles = "admin")]
    [HttpPost("add")]
    public async Task<IActionResult> Add([FromBody] ColorDto dto)
    {
        _db.Colors.Add(new Color { ColorName = dto.ColorName });
        await _db.SaveChangesAsync();
        return Ok(new ResponseModel { Success = true, Message = "Color added." });
    }

    [Authorize(Roles = "admin")]
    [HttpPost("updated")]
    public async Task<IActionResult> Update([FromBody] ColorDto dto)
    {
        var color = await _db.Colors.FindAsync(dto.ColorId);
        if (color is null) return NotFound(new ResponseModel { Success = false, Message = "Color not found." });
        color.ColorName = dto.ColorName;
        await _db.SaveChangesAsync();
        return Ok(new ResponseModel { Success = true, Message = "Color updated." });
    }

    [Authorize(Roles = "admin")]
    [HttpPost("delete")]
    public async Task<IActionResult> Delete([FromBody] ColorDto dto)
    {
        var color = await _db.Colors.FindAsync(dto.ColorId);
        if (color is null) return NotFound(new ResponseModel { Success = false, Message = "Color not found." });
        _db.Colors.Remove(color);
        await _db.SaveChangesAsync();
        return Ok(new ResponseModel { Success = true, Message = "Color deleted." });
    }
}
