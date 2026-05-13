using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentACar.DataAccess.Concrete.EntityFramework;

namespace RentACar.API.Controllers
{
    /// <summary>
    /// Temporary diagnostic controller — DELETE after the app is stable.
    /// Hit GET /api/diagnostic to see the current DB status in plain JSON.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class DiagnosticController : ControllerBase
    {
        private readonly RentACarContext _db;

        public DiagnosticController(RentACarContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var canConnect = await _db.Database.CanConnectAsync();

                int? carCount    = null;
                int? brandCount  = null;
                int? colorCount  = null;
                string? dbError  = null;

                if (canConnect)
                {
                    try { carCount   = await _db.Cars.CountAsync();   } catch (Exception ex) { dbError = ex.Message; }
                    try { brandCount = await _db.Brands.CountAsync(); } catch { /* ignore */ }
                    try { colorCount = await _db.Colors.CountAsync(); } catch { /* ignore */ }
                }

                return Ok(new
                {
                    canConnect,
                    carCount,
                    brandCount,
                    colorCount,
                    dbError,
                    database         = _db.Database.GetDbConnection().Database,
                    dataSource       = _db.Database.GetDbConnection().DataSource,
                    providerName     = _db.Database.ProviderName,
                    timestamp        = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error          = ex.Message,
                    innerException = ex.InnerException?.Message,
                    type           = ex.GetType().Name
                });
            }
        }
    }
}