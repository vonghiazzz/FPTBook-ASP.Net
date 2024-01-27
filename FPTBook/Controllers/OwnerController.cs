using FPTBook.Data;
using FPTBook.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml;

namespace FPTBook.Controllers
{
    [Authorize(Roles = "Owner")]
    public class OwnerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public OwnerController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            var ownerId = _userManager.GetUserId(User);
            var ownerName = _context.Users.FirstOrDefault(u => u.Id == ownerId)?.Name;
            var ownerPicture = _context.Users.FirstOrDefault(u => u.Id == ownerId)?.ProfilePicture;

            var ownerProduct = _context.Products.Where(p => p.OwnerId == ownerId).ToList();
            //get value in search form
            var searchTerm = Request.Query["search-term"];
            var searchOrderNumber = Request.Query["search-order-number"];

            var productSearch = _context.Products.Where(p => p.Name.Contains(searchTerm) && p.OwnerId == ownerId).ToList();
            var orderNumberSearch = (from o in _context.Orders
                                     join od in _context.OrderDetails
                                     on o.Id equals od.OrderId
                                     where o.Status == false && od.OwnerId == ownerId && o.Id.ToString() == searchOrderNumber.ToString()
                                     group new { o, od } by o.Id into grouped
                                     select new
                                     {
                                         OrderId = grouped.Key,
                                         TotalPrice = grouped.Sum(item => item.od.Total)
                                     }).ToList();

            if (!searchTerm.IsNullOrEmpty())
            {
                ViewData["ViewSearch"] = true;
            }
            else
            {
                ViewData["ViewSearch"] = false;
            }

            if(searchOrderNumber.IsNullOrEmpty())
            {
                ViewData["SearchOrderNumber"] = true;
            }
            else
            {
                ViewData["SearchOrderNumber"] = false;
            }

            //new genre
            var genreRequest = _context.NewGenres.Where(n=>n.OwnerId == ownerId).ToList();

            //Order
            var orderRecord = (from o in _context.Orders
                               join od in _context.OrderDetails
                               on o.Id equals od.OrderId
                               where o.Status == false && od.OwnerId == ownerId
                               group new { o, od } by o.Id into grouped
                               select new
                               {
                                   OrderId = grouped.Key,
                                   TotalPrice = grouped.Sum(item => item.od.Total)
                               }).ToList();

            ViewData["OwnerName"] = ownerName;
            ViewData["OwnerPicture"] = ownerPicture;
            ViewData["OwnerProduct"] = ownerProduct;
            ViewData["ProductSearch"] = productSearch;
            ViewData["OrderNumberSearch"] = orderNumberSearch;
            ViewData["GenreRequest"] = genreRequest;
            ViewData["OrderRecord"] = orderRecord;

            ViewBag.Authors = new SelectList(_context.Authorities, "Id", "Name");
            ViewBag.Genres = new SelectList(_context.Genres, "Id", "Name");
            ViewBag.Publishers = new SelectList(_context.Publishers, "Id", "Name");

            var data = _context.Products.Include(p => p.Genre)
                                        .Where(p=>p.OwnerId == ownerId)
                                       .GroupBy(p => p.Genre.Name)
                                       .Select(g => new { Title = g.Key, Total = g.Count() }).ToList();

            string[] labels = new string[data.Count];
            string[] totals = new string[data.Count];

            for (int i = 0; i < data.Count; i++)
            {
                labels[i] = data[i].Title;
                totals[i] = data[i].Total.ToString();
            }

            ViewData["labels"] = string.Format("'{0}'", String.Join("','", labels));
            ViewData["totals"] = String.Join(",", totals);



            var data1 = _context.OrderDetails.Include(os => os.Order)
                                        .Where(os => os.Order.Status == true && os.OwnerId == ownerId)
                                        .GroupBy(os => os.Order.OrderDate)
                                        .Select(k => new {
                                            Title = k.Key,
                                            Total = k.Sum(od => od.Total)
                                        }).ToList();
            /* Total = k.SelectMany(os => os.OrderDetails).Sum(od => od.Total)
                                         }).ToList();*/

            string[] labels1 = new string[data1.Count];
            string[] totals1 = new string[data1.Count];

            for (int i = 0; i < data1.Count; i++)
            {

                labels1[i] = String.Format("{0:MM/dd/yyyy}", (data1[i].Title));
                totals1[i] = data1[i].Total.ToString();
            }

            ViewData["labels1"] = string.Format("'{0}'", String.Join("','", labels1));
            ViewData["totals1"] = String.Join(",", totals1);

            return View("/Views/Owner/OwnerScreen.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> RequestNewGenre(string genreReq)
        {
            var ownerId = _userManager.GetUserId(User);
            NewGenre newGenre = new NewGenre
            {
                Name = genreReq,
                OwnerId = ownerId,
                Status = null
            };

            _context.Add(newGenre);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public IActionResult OrderRecordDetail(int id)
        {
            var orderDetail = (from o in _context.Orders
                               where o.Id == id
                               join od in _context.OrderDetails
                               on o.Id equals od.OrderId
                               join c in _context.Users
                               on o.CustomerId equals c.Id
                               group new { o, od, c } by o.Id into grouped
                               select new
                               {
                                   OrderId = grouped.Key,
                                   Customer = grouped.Select(item => item.c).FirstOrDefault(),
                                   TotalPrice = grouped.Sum(item => item.od.Total)
                               }).FirstOrDefault();

            var orderMoreDetail = (from o in _context.Orders
                                   where o.Id == id
                                   join od in _context.OrderDetails on o.Id equals od.OrderId
                                   join p in _context.Products on od.ProductId equals p.Id
                                   select new
                                   {
                                       OrderDetail = od,
                                       Product = p
                                   }).ToList();
            ViewData["OrderDetail"] = orderDetail;
            ViewData["OrderMoreDetail"] = orderMoreDetail;
            ViewData["OrderId"] = id;
            return View("Views/Owner/ConfirmOrder.cshtml");
        }

        public async Task<IActionResult> ConfirmOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            order.Status = true;
            _context.Update(order);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public IActionResult Export()
        {
            var ownerId = _userManager.GetUserId(User);
            List<Product> list = _context.Products.Include(p => p.Author).Include(p => p.Genre).Include(p => p.Publisher).Where(p=>p.OwnerId == ownerId).ToList<Product>();

            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                var sheet = package.Workbook.Worksheets.Add("Information book");
                sheet.Cells["A1"].Value = "Books list";
                sheet.Cells["A3"].Value = "Id";
                sheet.Cells["B3"].Value = "Name";
                sheet.Cells["C3"].Value = "Category";
                sheet.Cells["D3"].Value = "Page Number";
                sheet.Cells["E3"].Value = "Author";
                sheet.Cells["F3"].Value = "Price";
                sheet.Cells["G3"].Value = "Quantity";
                sheet.Cells["H3"].Value = "Publisher";
                sheet.Cells["I3"].Value = "Publish Date";
                sheet.Cells["J3"].Value = "Language";
                sheet.Cells["K3"].Value = "Description";



                int row = 4;
                foreach (var p in list)
                {
                    sheet.Cells[row, 1].Value = p.Id;
                    sheet.Cells[row, 2].Value = p.Name;
                    sheet.Cells[row, 3].Value = p.Genre.Name;
                    sheet.Cells[row, 4].Value = p.PageNumber;
                    sheet.Cells[row, 5].Value = p.Author.Name;
                    sheet.Cells[row, 6].Value = p.Price;
                    sheet.Cells[row, 7].Value = p.Quantity;
                    sheet.Cells[row, 8].Value = p.Publisher.Name;
                    sheet.Cells[row, 9].Value = p.PublishDate;
                    sheet.Cells[row, 10].Value = p.Language;
                    sheet.Cells[row, 11].Value = p.Description;


                    row++;
                }
                package.Save();
            }
            stream.Position = 0;
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Fptbooks.xlsx");
        }
    }
}
