using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using RapidPay.DAL;
using RapidPay.DAL.Models;
using RapidPay.Service.DTO;
using RapidPay.Service.Helper;
using RapidPay.Shared.Utils;

namespace RapidPay.Service.Services.Card
{
    public class CardService : ICardService
    {
        private readonly ContextDB _context;
        private readonly FileLogger _Logger;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;


        public CardService(ContextDB context, FileLogger fileLogger, IMapper mapper, IMemoryCache cache)
        {
            _context = context;
            _Logger = fileLogger;
            _mapper = mapper;
            _cache = cache;
        }
        public async Task<CreateCardResponse> CreateCardAsync(CreateCardRequest request)
        {
            if (request.CardNumber.Length != 15)
            {
                string error = "Card number must be 15 digits.";
                await _Logger.LogAsync($"Card creation process failed for: '{error}'.");
                throw new ArgumentException(error);
            }

            // Check if the card number already exists
            var existingCard = await _context.Cards.FirstOrDefaultAsync(c => c.CardNumber == request.CardNumber);
            if (existingCard != null)
            {
                string error = "Card number already exists.";
                await _Logger.LogAsync($"Card creation process failed for: '{error}'.");
                throw new ArgumentException(error);
            }

            DAL.Models.Card card = new DAL.Models.Card { CardNumber = request.CardNumber, Balance = request.Balance };
            _context.Cards.Add(card);
            await _context.SaveChangesAsync();

            CreateCardResponse mappedCard = _mapper.Map<CreateCardResponse>(card);
            return mappedCard;
        }

        public async Task<decimal> GetCardBalanceAsync(string cardNumber)
        {
            var card = await _context.Cards.FirstOrDefaultAsync(c => c.CardNumber == cardNumber);
            if (card == null)
            {
                string error = "Card not found.";
                await _Logger.LogAsync($"Get card balance process failed for: '{error}'.");
                throw new Exception(error);
            }

            return card.Balance;
        }

        public async Task<PaymentResponse> PayAsync(PaymentRequest request)
        {
            // Call the Initialize method first
            UFE.Initialize(_cache, _Logger);

            // Fetch the card details asynchronously
            var card = await _context.Cards.FirstOrDefaultAsync(c => c.CardNumber == request.CardNumber);

            if (card == null)
            {
                string error = "Card not found.";
                await _Logger.LogAsync($"Payment process failed for: '{error}'.");
                throw new ArgumentException("Card not found.");
            }

            // Get the current fee asynchronously
            var fee = await Task.Run(() => UFE.Instance.GetCurrentFee());
            var totalAmount = request.Amount + request.Amount * fee;

            // Ensure atomicity and integrity using a transaction
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Check if the balance is sufficient
                    if (card.Balance < totalAmount)
                    {
                        string error = "Insufficient balance.";
                        await _Logger.LogAsync($"Payment process failed for: '{error}'.");
                        throw new Exception("Insufficient balance.");
                    }

                    // Deduct the amount from the balance
                    card.Balance -= totalAmount;

                    // Create the payment record
                    var payment = new Payment
                    {
                        CardId = card.Id,
                        Amount = request.Amount,
                        PaymentDate = DateTime.Now
                    };

                    _context.Payments.Add(payment);

                    // Save the changes to the database
                    await _context.SaveChangesAsync();

                    // Commit the transaction
                    await transaction.CommitAsync();

                    // Map the payment to PaymentResponse
                    var mappedPayment = _mapper.Map<PaymentResponse>(payment);
                    return mappedPayment;
                }
                catch (Exception ex)
                {
                    // Rollback the transaction in case of an error
                    await transaction.RollbackAsync();
                    await _Logger.LogAsync($"Payment process failed for: '{ex.Message}'.");
                    throw;
                }
            }
        }
    }
}
