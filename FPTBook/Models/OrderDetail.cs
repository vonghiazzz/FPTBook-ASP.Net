using System.ComponentModel.DataAnnotations.Schema;

namespace FPTBook.Models
{
    public class OrderDetail
    {
        public int OrderId { get; set; }
        public Order? Order { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }

        [Column(TypeName = "nvarchar(450)")]
        public string? OwnerId { get; set; }

        public int? Quantity { get; set; }
        public float? Total { get; set; }
    }
}
