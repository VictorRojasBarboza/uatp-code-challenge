using AutoMapper;
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
        private readonly IMapper _mapper;
        public CardController(ICardService cardService, FileLogger fileLogger, IMapper mapper)
        {
            _cardService = cardService;
            _Logger = fileLogger;
            _mapper = mapper;
        }

        [HttpPost("create")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> CreateCard([FromBody] DTO.CreateCardRequest request)
        {
            try
            {
                // Map the request DTO to the service DTO
                var serviceRequest = _mapper.Map<Service.DTO.CreateCardRequest>(request);

                var result = await _cardService.CreateCardAsync(serviceRequest);
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
        public async Task<IActionResult> Pay([FromBody] DTO.PaymentRequest request)
        {
            try
            {
                // Map the request DTO to the service DTO
                var serviceRequest = _mapper.Map<Service.DTO.PaymentRequest>(request);

                var result = await _cardService.PayAsync(serviceRequest);
                await _Logger.LogAsync($"Payment ${result.Id} was succeeded registered.");
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
                return Ok($"Balance for card {cardNumber} is :${balance}");
            }
            catch (Exception ex)
            {
                await _Logger.LogAsync($"Get Balance process failed for:  {ex.Message}.");
                return BadRequest(ex.Message);
            }
        }
    }
}
