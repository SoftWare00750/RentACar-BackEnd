using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentACar.API.Data;
using RentACar.API.DTOs;

namespace RentACar.API.Controllers;

[ApiController]
[Route("api/creditcards")]
[Authorize]
public class CreditCardsController : ControllerBase
{
    private readonly AppDbContext _db;
    public CreditCardsController(AppDbContext db) => _db = db;

    /// <summary>
    /// Checks whether the card exists and has matching details.
    /// Returns success:true if the card is valid.
    /// </summary>
    [HttpPost("iscardexist")]
    public async Task<IActionResult> IsCardExist([FromBody] CreditCardCheckRequest req)
    {
        // Normalize card number (remove spaces)
        var cardNo = req.CardNumber.Replace(" ", "");

        var card = await _db.CreditCards.FirstOrDefaultAsync(c =>
            c.CardNumber     == cardNo &&
            c.CardCvc        == req.CardCvc &&
            c.CardExpiration == req.CardExpiration);

        if (card is null)
            return Ok(new ResponseModel { Success = false, Message = "Card not found or details do not match." });

        if (card.MoneyInTheCard <= 0)
            return Ok(new ResponseModel { Success = false, Message = "Insufficient funds on card." });

        return Ok(new ResponseModel { Success = true, Message = "Card verified." });
    }

    /// <summary>Get card details by card number (for balance deduction)</summary>
    [HttpGet("getbycardnumber")]
    public async Task<IActionResult> GetByCardNumber([FromQuery] string cardnumber)
    {
        var cardNo = cardnumber.Replace(" ", "");
        var cards  = await _db.CreditCards
            .Where(c => c.CardNumber == cardNo)
            .Select(c => new CreditCardDto
            {
                Id             = c.Id,
                CardName       = c.CardName,
                CardNumber     = c.CardNumber,
                CardCvc        = c.CardCvc,
                CardExpiration = c.CardExpiration,
                MoneyInTheCard = c.MoneyInTheCard
            })
            .ToListAsync();

        return Ok(new ListResponseModel<CreditCardDto> { Success = true, Message = "OK.", Data = cards });
    }

    /// <summary>Update card balance after a rental payment</summary>
    [HttpPut("update")]
    public async Task<IActionResult> UpdateCard([FromBody] CreditCardDto dto)
    {
        var card = await _db.CreditCards.FindAsync(dto.Id);
        if (card is null) return NotFound(new ResponseModel { Success = false, Message = "Card not found." });

        card.MoneyInTheCard = dto.MoneyInTheCard;
        await _db.SaveChangesAsync();

        return Ok(new ResponseModel { Success = true, Message = "Card balance updated." });
    }
}
