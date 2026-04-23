using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentACar.Business.Abstract;
using RentACar.Entities.Concrete;

namespace RentACar.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RentalsController : ControllerBase
    {
        IRentalService _rentalService;

        public RentalsController(IRentalService rentalService)
        {
            _rentalService = rentalService;
        }

        // /rentals/getall
        [HttpGet("getall")]
        public IActionResult GetAll()
        {
            var result = _rentalService.GetAll();
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        // /rentals/getallrentaldto  (frontend rental service uses this)
        [HttpGet("getallrentaldto")]
        public IActionResult GetAllRentalDto()
        {
            var result = _rentalService.GetAll();
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        // /rentals/add  (frontend adds rental after credit card)
        [HttpPost("add")]
        public IActionResult Add([FromBody] Rental rental)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // CustomerId defaults to 0 if not set (frontend doesn't send it currently)
            if (rental.CustomerId == 0) rental.CustomerId = 1;

            var result = _rentalService.Add(rental);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        // /rentals/isrentable  (frontend checks availability)
        [HttpPost("isrentable")]
        public IActionResult IsRentable([FromBody] Rental rental)
        {
            // Check if the car is available for rental
            var allRentals = _rentalService.GetAll();
            if (!allRentals.Success)
                return Ok(new { success = true, message = "Car is available" });

            bool hasConflict = allRentals.Data.Any(r =>
                r.CarId == rental.CarId &&
                r.ReturnDate == null);

            if (hasConflict)
                return Ok(new { success = false, message = "Car is currently rented out" });

            return Ok(new { success = true, message = "Car is available" });
        }

        [HttpPut("update")]
        public IActionResult Update([FromBody] Rental rental)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = _rentalService.Update(rental);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpDelete("delete/{rentalId}")]
        public IActionResult Delete(int rentalId)
        {
            var rental = _rentalService.GetAll().Data?.FirstOrDefault(r => r.RentalId == rentalId);
            if (rental == null) return NotFound(new { Message = "Rental not found" });
            var result = _rentalService.Delete(rental);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }
    }
}