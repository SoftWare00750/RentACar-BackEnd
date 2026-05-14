using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentACar.API.Data;
using RentACar.API.DTOs;
using RentACar.API.Models;

namespace RentACar.API.Controllers;

[ApiController]
[Route("api/brands")]
public class BrandsController : ControllerBase
{
    private readonly AppDbContext _db;
    public BrandsController(AppDbContext db) => _db = db;

    [HttpGet("getall")]
    public async Task<IActionResult> GetAll()
    {
        var data = await _db.Brands
            .OrderBy(b => b.BrandName)
            .Select(b => new BrandDto { BrandId = b.BrandId, BrandName = b.BrandName })
            .ToListAsync();
        return Ok(new ListResponseModel<BrandDto> { Success = true, Message = "Brands listed.", Data = data });
    }

    [HttpGet("getbyid")]
    public async Task<IActionResult> GetById([FromQuery] int brandId)
    {
        var brand = await _db.Brands.FindAsync(brandId);
        if (brand is null) return NotFound(new ResponseModel { Success = false, Message = "Brand not found." });
        return Ok(new SingleResponseModel<BrandDto>
        {
            Success = true, Message = "OK.",
            Data    = new BrandDto { BrandId = brand.BrandId, BrandName = brand.BrandName }
        });
    }

    [Authorize(Roles = "admin")]
    [HttpPost("add")]
    public async Task<IActionResult> Add([FromBody] BrandDto dto)
    {
        _db.Brands.Add(new Brand { BrandName = dto.BrandName });
        await _db.SaveChangesAsync();
        return Ok(new ResponseModel { Success = true, Message = "Brand added." });
    }

    [Authorize(Roles = "admin")]
    [HttpPost("updated")]
    public async Task<IActionResult> Update([FromBody] BrandDto dto)
    {
        var brand = await _db.Brands.FindAsync(dto.BrandId);
        if (brand is null) return NotFound(new ResponseModel { Success = false, Message = "Brand not found." });
        brand.BrandName = dto.BrandName;
        await _db.SaveChangesAsync();
        return Ok(new ResponseModel { Success = true, Message = "Brand updated." });
    }

    [Authorize(Roles = "admin")]
    [HttpPost("delete")]
    public async Task<IActionResult> Delete([FromBody] BrandDto dto)
    {
        var brand = await _db.Brands.FindAsync(dto.BrandId);
        if (brand is null) return NotFound(new ResponseModel { Success = false, Message = "Brand not found." });
        _db.Brands.Remove(brand);
        await _db.SaveChangesAsync();
        return Ok(new ResponseModel { Success = true, Message = "Brand deleted." });
    }
}
