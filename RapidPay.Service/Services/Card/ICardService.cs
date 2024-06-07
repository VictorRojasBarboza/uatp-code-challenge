using RapidPay.Service.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RapidPay.Service.Services.Card
{
    public interface ICardService
    {
        Task<string> CreateCardAsync(string cardNumber);
        Task<string> PayAsync(PaymentRequest request);
        Task<decimal> GetCardBalanceAsync(string cardNumber);
    }
}
