﻿using Microsoft.EntityFrameworkCore;
using RapidPay.DAL;
using RapidPay.DAL.Models;
using RapidPay.Service.DTO;
using RapidPay.Service.Helper;
using RapidPay.Shared.Utils;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RapidPay.Service.Services.Card
{
    public class CardService : ICardService
    {
        private readonly ContextDB _context;
        private readonly FileLogger _Logger;

        public CardService(ContextDB context, FileLogger fileLogger)
        {
            _context = context;
            _Logger = fileLogger;
        }
        public async Task<string> CreateCardAsync(string cardNumber)
        {
            if (cardNumber.Length != 15)
            {
                string error = "Card number must be 15 digits.";
                await _Logger.LogAsync($"Card creation process failed for: '{error}'.");
                throw new ArgumentException(error);
            }

            // Check if the card number already exists
            var existingCard = await _context.Cards.FirstOrDefaultAsync(c => c.CardNumber == cardNumber);
            if (existingCard != null)
            {
                string error = "Card number already exists.";
                await _Logger.LogAsync($"Card creation process failed for: '{error}'.");
                throw new ArgumentException(error);
            }

            var card = new DAL.Models.Card { CardNumber = cardNumber, Balance = 0 };
            _context.Cards.Add(card);
            await _context.SaveChangesAsync();

            return "Card created successfully.";
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

        public async Task<string> PayAsync(PaymentRequest request)
        {
            //Thread implementation for database query
            var cardTask = Task.Run(async () =>
            {
                return await _context.Cards.FirstOrDefaultAsync(c => c.CardNumber == request.CardNumber);
            });

            var card = await cardTask; 
            
            if (card == null)
            {
                string error = "Card not found.";
                await _Logger.LogAsync($"Payment process failed for: '{error}'.");
                throw new Exception("Card not found.");
            }

            //Thread implementation to get Fee calculated value.
            var feeTask = Task.Run(() => UFE.Instance.GetCurrentFee());
            var fee = await feeTask;

            var totalAmount = request.Amount + request.Amount * fee;

            //Thread implementation for balance and payment processes
            var updateBalanceTask = Task.Run(() =>
            {
                card.Balance -= totalAmount;
            });

            var paymentTask = Task.Run(() =>
            {
                var payment = new Payment
                {
                    CardId = card.Id,
                    Amount = request.Amount,
                    PaymentDate = DateTime.Now
                };

                _context.Payments.Add(payment);
            });

            //Ensures that all parallel tasks are completed before proceeding
            await Task.WhenAll(updateBalanceTask, paymentTask);
            await _context.SaveChangesAsync();

            return "Payment successful.";
        }
    }
}