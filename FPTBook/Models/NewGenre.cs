using FPTBook.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace FPTBook.Models
{
    public class NewGenre
    {
        public int Id { get; set; }
        [Column(TypeName = "nvarchar(450)")]
        public string OwnerId { get; set; }
        public ApplicationUser? Owner { get; set; }
        public string? Name { get; set; }
        public bool? Status { get; set; }
    }
}
