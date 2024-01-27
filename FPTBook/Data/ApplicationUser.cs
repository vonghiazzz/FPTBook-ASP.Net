using FPTBook.Models;
using Microsoft.AspNetCore.Identity;

namespace FPTBook.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? ProfilePicture { get; set; }
        public ICollection<Order>? Orders { get; set; }
        public ICollection<Cart>? Carts { get; set; }
        public ICollection<NewGenre>? NewGenres { get; set; }
    }
}
