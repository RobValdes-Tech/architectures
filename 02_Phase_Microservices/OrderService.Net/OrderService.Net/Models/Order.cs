using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderService.Net.Models
{
    [Table("orders")]
    public class Order
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [Column("user_id")]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [Column("product_id")]
        public string ProductId { get; set; } = string.Empty;

        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
