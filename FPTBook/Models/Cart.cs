using FPTBook.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace FPTBook.Models
{
    public class Cart
    {
        public int Id { get; set; }
        [Column(TypeName = "nvarchar(450)")]
        public string CustomerId { get; set; }
        public ApplicationUser? Customer { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public int? Quantity { get; set; }



    }
}
