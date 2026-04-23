using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentACar.Business.Abstract;
using RentACar.Entities.Concrete;

namespace RentACar.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ColorsController : ControllerBase
    {
        IColorService _colorService;

        public ColorsController(IColorService colorService)
        {
            _colorService = colorService;
        }

        [HttpGet("getall")]
        [AllowAnonymous]
        public IActionResult GetAll()
        {
            var result = _colorService.GetAll();
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        // /colors/getbyid?colorId=5
        [HttpGet("getbyid")]
        [AllowAnonymous]
        public IActionResult GetById([FromQuery] int colorId)
        {
            var result = _colorService.GetById(colorId);
            if (result.Success) return Ok(result);
            return NotFound(result);
        }

        // /colors/getbyid/{colorId}
        [HttpGet("getbyid/{colorId:int}")]
        [AllowAnonymous]
        public IActionResult GetByIdRoute([FromRoute] int colorId)
        {
            var result = _colorService.GetById(colorId);
            if (result.Success) return Ok(result);
            return NotFound(result);
        }

        [HttpPost("add")]
        public IActionResult Add([FromBody] Color color)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = _colorService.Add(color);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        // Frontend sends POST to /colors/updated
        [HttpPost("updated")]
        public IActionResult UpdatedPost([FromBody] Color color)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = _colorService.Update(color);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpPut("update")]
        public IActionResult Update([FromBody] Color color)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = _colorService.Update(color);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        // Frontend sends POST to /colors/delete
        [HttpPost("delete")]
        public IActionResult DeletePost([FromBody] Color color)
        {
            var result = _colorService.Delete(color);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpDelete("delete/{colorId}")]
        public IActionResult Delete(int colorId)
        {
            var color = _colorService.GetById(colorId);
            if (!color.Success) return NotFound(color);
            var result = _colorService.Delete(color.Data);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }
    }
}