using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentACar.Business.Abstract;
using RentACar.Entities.Concrete;

namespace RentACar.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CarsController : ControllerBase
    {
        ICarService _carService;

        public CarsController(ICarService carService)
        {
            _carService = carService;
        }

        [HttpGet("getall")]
        [AllowAnonymous]
        public IActionResult GetAll()
        {
            var result = _carService.GetAll();
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet("getbyid/{carId}")]
        [AllowAnonymous]
        public IActionResult GetById(int carId)
        {
            var result = _carService.GetById(carId);
            if (result.Success)
            {
                return Ok(result);
            }
            return NotFound(result);
        }

        [HttpGet("getcardetails")]
        [AllowAnonymous]
        public IActionResult GetCarDetails()
        {
            var result = _carService.GetCarDetails();
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet("getcardetail/{carId}")]
        [AllowAnonymous]
        public IActionResult GetCarDetail(int carId)
        {
            var result = _carService.GetCarDetail(carId);
            if (result.Success)
            {
                return Ok(result);
            }
            return NotFound(result);
        }

        [HttpGet("getbybrand/{brandId}")]
        [AllowAnonymous]
        public IActionResult GetByBrandId(int brandId)
        {
            var result = _carService.GetCarsByBrandId(brandId);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet("getbycolor/{colorId}")]
        [AllowAnonymous]
        public IActionResult GetByColorId(int colorId)
        {
            var result = _carService.GetCarsByColorId(colorId);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("add")]
        public IActionResult Add([FromBody] Car car)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = _carService.Add(car);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPut("update")]
        public IActionResult Update([FromBody] Car car)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = _carService.Update(car);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpDelete("delete/{carId}")]
        public IActionResult Delete(int carId)
        {
            var car = _carService.GetById(carId);
            if (!car.Success)
            {
                return NotFound(car);
            }
            
            var result = _carService.Delete(car.Data);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}