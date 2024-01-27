using System.ComponentModel.DataAnnotations.Schema;

namespace FPTBook.Models
{
    public class Author
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        [Column(TypeName = "nvarchar(max)")]
        public string? Description { get; set; }
        public string? Picture { get; set; }
        [NotMapped]
        public IFormFile? ImageFile { get; set; }
        public ICollection<Product>? Products { get; set; }
    }
}
