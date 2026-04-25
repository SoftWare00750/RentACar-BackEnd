using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentACar.Business.Abstract;
using RentACar.Entities.Concrete;
using RentACar.Entities.DTOs;

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
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        // Frontend calls: cars/getcardetails
        [HttpGet("getcardetails")]
        [AllowAnonymous]
        public IActionResult GetCarDetails()
        {
            var result = _carService.GetCarDetails();
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        // Frontend calls: cars/getcardetail/{carId} OR cars/getcardetail?carId=X
        [HttpGet("getcardetail/{carId}")]
        [AllowAnonymous]
        public IActionResult GetCarDetail(int carId)
        {
            var result = _carService.GetCarDetail(carId);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpGet("getcardetail")]
        [AllowAnonymous]
        public IActionResult GetCarDetailQuery([FromQuery] int carId)
        {
            var result = _carService.GetCarDetail(carId);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        // Frontend car.service.ts calls: cars/getbyid?carId=X
        [HttpGet("getbyid")]
        [AllowAnonymous]
        public IActionResult GetById([FromQuery] int carId)
        {
            var result = _carService.GetById(carId);
            if (result.Success) return Ok(result);
            return NotFound(result);
        }

        [HttpGet("getbyid/{carId:int}")]
        [AllowAnonymous]
        public IActionResult GetByIdRoute([FromRoute] int carId)
        {
            var result = _carService.GetById(carId);
            if (result.Success) return Ok(result);
            return NotFound(result);
        }

        // Frontend car.service.ts calls: cars/getcarsbybrandid?brandId=X
        [HttpGet("getcarsbybrandid")]
        [AllowAnonymous]
        public IActionResult GetCarsByBrandId([FromQuery] int brandId)
        {
            var result = _carService.GetCarsByBrandId(brandId);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        // Frontend car.service.ts calls: cars/getcarsbycolorid?colorId=X
        [HttpGet("getcarsbycolorid")]
        [AllowAnonymous]
        public IActionResult GetCarsByColorId([FromQuery] int colorId)
        {
            var result = _carService.GetCarsByColorId(colorId);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        // Frontend car.service.ts calls: cars/getcarsbybrandandcolorid?brandId=X&colorId=Y
        [HttpGet("getcarsbybrandandcolorid")]
        [AllowAnonymous]
        public IActionResult GetCarsByBrandAndColorId([FromQuery] int brandId, [FromQuery] int colorId)
        {
            var result = _carService.GetCarsByBrandAndColorId(brandId, colorId);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        // Legacy aliases kept for backward compatibility with old frontend routes
        [HttpGet("getbybrand")]
        [AllowAnonymous]
        public IActionResult GetByBrand([FromQuery] int brandId)
        {
            var result = _carService.GetCarsByBrandId(brandId);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpGet("getbycolor")]
        [AllowAnonymous]
        public IActionResult GetByColor([FromQuery] int colorId)
        {
            var result = _carService.GetCarsByColorId(colorId);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpGet("getbyselected")]
        [AllowAnonymous]
        public IActionResult GetBySelected([FromQuery] int brandId, [FromQuery] int colorId)
        {
            var result = _carService.GetCarsByBrandAndColorId(brandId, colorId);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        // Frontend admin calls: cars/getallcardetail
        [HttpGet("getallcardetail")]
        [AllowAnonymous]
        public IActionResult GetAllCarDetail()
        {
            var result = _carService.GetCarDetails();
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("add")]
        public IActionResult Add([FromBody] Car car)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = _carService.Add(car);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        // Frontend car-edit calls POST to cars/update
        [HttpPost("update")]
        public IActionResult UpdatePost([FromBody] Car car)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = _carService.Update(car);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("updated")]
        public IActionResult UpdatedPost([FromBody] Car car)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = _carService.Update(car);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpPut("update")]
        public IActionResult Update([FromBody] Car car)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = _carService.Update(car);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("delete")]
        public IActionResult DeletePost([FromBody] Car car)
        {
            var result = _carService.Delete(car);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpDelete("delete/{carId}")]
        public IActionResult Delete(int carId)
        {
            var car = _carService.GetById(carId);
            if (!car.Success) return NotFound(car);
            var result = _carService.Delete(car.Data);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }
    }
}