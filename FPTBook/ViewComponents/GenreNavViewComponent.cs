using FPTBook.Data;
using Microsoft.AspNetCore.Mvc;

namespace FPTBook.ViewComponents
{
    public class GenreNavViewComponent :ViewComponent
    {
        private readonly ApplicationDbContext _context;
        public GenreNavViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var genreList = _context.Genres.ToList();

            return View(genreList);
        }
    }
}
