using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentACar.Business.Abstract;
using RentACar.Entities.Concrete;

namespace RentACar.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BrandsController : ControllerBase
    {
        IBrandService _brandService;

        public BrandsController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        [HttpGet("getall")]
        [AllowAnonymous]
        public IActionResult GetAll()
        {
            var result = _brandService.GetAll();
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        // /brands/getbyid?brandId=5
        [HttpGet("getbyid")]
        [AllowAnonymous]
        public IActionResult GetById([FromQuery] int brandId)
        {
            var result = _brandService.GetById(brandId);
            if (result.Success) return Ok(result);
            return NotFound(result);
        }

        // /brands/getbyid/{brandId}
        [HttpGet("getbyid/{brandId:int}")]
        [AllowAnonymous]
        public IActionResult GetByIdRoute([FromRoute] int brandId)
        {
            var result = _brandService.GetById(brandId);
            if (result.Success) return Ok(result);
            return NotFound(result);
        }

        [HttpPost("add")]
        public IActionResult Add([FromBody] Brand brand)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = _brandService.Add(brand);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        // Frontend sends POST to /brands/updated
        [HttpPost("updated")]
        public IActionResult UpdatedPost([FromBody] Brand brand)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = _brandService.Update(brand);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpPut("update")]
        public IActionResult Update([FromBody] Brand brand)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = _brandService.Update(brand);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        // Frontend sends POST to /brands/delete
        [HttpPost("delete")]
        public IActionResult DeletePost([FromBody] Brand brand)
        {
            var result = _brandService.Delete(brand);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpDelete("delete/{brandId}")]
        public IActionResult Delete(int brandId)
        {
            var brand = _brandService.GetById(brandId);
            if (!brand.Success) return NotFound(brand);
            var result = _brandService.Delete(brand.Data);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }
    }
}