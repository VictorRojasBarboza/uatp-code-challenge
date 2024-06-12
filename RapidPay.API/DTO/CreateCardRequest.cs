namespace RapidPay.API.DTO
{
    public class CreateCardRequest
    {
        public string CardNumber { get; set; }
        public decimal Balance { get; set; }
    }
}
