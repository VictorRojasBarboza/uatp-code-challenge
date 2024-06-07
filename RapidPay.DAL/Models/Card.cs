using System.ComponentModel.DataAnnotations;

namespace RapidPay.DAL.Models
{
    public class Card
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(15, MinimumLength = 15)]
        public string CardNumber { get; set; }

        [Required]
        public decimal Balance { get; set; }
    }
}
