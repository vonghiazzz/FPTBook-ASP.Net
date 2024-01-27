using FPTBook.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FPTBook.Models
{
    public class Order
    {
        public int Id { get; set; }
        [Column(TypeName = "nvarchar(450)")]
        public string CustomerId { get; set; }
        public ApplicationUser? Customer { get; set; }
        [Column(TypeName = "nvarchar(450)")]
        public string? OwnerId { get; set; }
        [DataType(DataType.Date)]
        public DateTime? OrderDate { get; set; }

        public int? Total { get; set; }
        public bool? Status { get; set; }
        public ICollection<OrderDetail>? OrderDetails { get; set; }
    }
}
