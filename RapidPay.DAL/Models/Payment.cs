using System.ComponentModel.DataAnnotations;

namespace RapidPay.DAL.Models
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CardId { get; set; }
        public Card Card { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public DateTime PaymentDate { get; set; }
    }
}
