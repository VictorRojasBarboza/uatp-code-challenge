using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RapidPay.Service.DTO
{
    public class PaymentRequest
    {
        public string CardNumber { get; set; }
        public decimal Amount { get; set; }
    }
}
