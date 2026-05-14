using Microsoft.AspNetCore.Mvc;
using RentACar.API.Data;
using RentACar.API.DTOs;
using RentACar.API.Models;

namespace RentACar.API.Controllers;

/// <summary>
/// Stub controller to satisfy the frontend CustomerService.
/// Returns Users cast as "customers".
/// </summary>
[ApiController]
[Route("api/customers")]
public class CustomersController : ControllerBase
{
    private readonly AppDbContext _db;
    public CustomersController(AppDbContext db) => _db = db;

    [HttpGet("getcustomerdetails")]
    public IActionResult GetCustomerDetails()
    {
        // Returns an empty list — the Angular template has the row content commented out
        return Ok(new ListResponseModel<object>
        {
            Success = true,
            Message = "Customers listed.",
            Data    = new List<object>()
        });
    }
}
