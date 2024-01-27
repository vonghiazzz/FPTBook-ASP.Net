using FPTBook.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Security.Policy;

namespace FPTBook.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int? Quantity { get; set; }
        public float? Price { get; set; }
        public string? Picture { get; set; }
        [NotMapped]
        public IFormFile? ImageFile { get; set; }
        [DataType(DataType.Date)]
        public DateTime? PublishDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime? ImportDate { get; set; }
        public int? PageNumber { get; set; }
        public string? Language { get; set; }
        [Column(TypeName = "nvarchar(max)")]
        public string? Description { get; set; }
        public bool? Status { get; set; }
        public int AuthorId { get; set; }
        public Author? Author { get; set; }
        public int GenreId { get; set; }
        public Genre? Genre { get; set; }
        public int PublisherId { get; set; }
        public Publisher? Publisher { get; set; }
        [Column(TypeName = "nvarchar(450)")]
        public string? OwnerId { get; set; }
        public ICollection<OrderDetail>? OrderDetails { get; set; }
        public ICollection<Cart>? Carts { get; set; }
    }
}
