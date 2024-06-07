using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RapidPay.Service.DTO;
using RapidPay.Service.Services.Card;
using RapidPay.Shared.Utils;

namespace RapidPay.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CardController : ControllerBase
    {
        private readonly ICardService _cardService;
        private readonly FileLogger _Logger;
        public CardController(ICardService cardService, FileLogger fileLogger)
        {
            _cardService = cardService;
            _Logger = fileLogger;
        }

        [HttpPost("create")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> CreateCard([FromBody] CreateCardRequest request)
        {
            try
            {
                var result = await _cardService.CreateCardAsync(request.CardNumber);
                await _Logger.LogAsync($"Card {request.CardNumber} created succeeded.");
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                await _Logger.LogAsync($"Card creation process failed for:  {ex.Message}.");
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                await _Logger.LogAsync($"Card creation process failed for:  {ex.Message}.");
                return Conflict(ex.Message);
            }
        }

        [HttpPost("pay")]
        [Authorize(Policy = "RequireUserRole")]
        public async Task<IActionResult> Pay([FromBody] PaymentRequest request)
        {
            try
            {
                var result = await _cardService.PayAsync(request);
                await _Logger.LogAsync($"Payment was succeeded registered.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                await _Logger.LogAsync($"Payment process failed for:  {ex.Message}.");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("balance")]
        [Authorize(Policy = "RequireUserRole")]
        public async Task<IActionResult> GetCardBalance([FromQuery] string cardNumber)
        {
            try
            {
                var balance = await _cardService.GetCardBalanceAsync(cardNumber);
                return Ok(balance);
            }
            catch (Exception ex)
            {
                await _Logger.LogAsync($"Get Balance process failed for:  {ex.Message}.");
                return BadRequest(ex.Message);
            }
        }
    }
}
