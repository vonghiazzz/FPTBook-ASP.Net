using FPTBook.Data;
using FPTBook.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace FPTBook.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _logger = logger;
            _context = context;

            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Admin");
            }

            if (User.IsInRole("Owner"))
            {
                return RedirectToAction("Index", "Owner");
            }

            var newRelease = _context.Products.OrderByDescending(p=>p.ImportDate).Take(15).ToList();

            var bestSeller = (from p in _context.Products
                               join od in _context.OrderDetails on p.Id equals od.ProductId
                               group new { p, od } by p into grouped
                               select new
                               {
                                   Product = grouped.Key,
                                   TotalQuantity = grouped.Sum(x => x.od.Quantity)
                               })
                  .OrderByDescending(x => x.TotalQuantity)
                  .Take(15)
                  .Select(x => x.Product)
                  .ToList();

            ViewData["NewRelease"] = newRelease;
            ViewData["BestSeller"] = bestSeller;
            return View();
        }

        public IActionResult BookDetail(int id)
        {
            //get book info
            var detail = (from product in _context.Products
                          join author in _context.Authorities
                          on product.AuthorId equals author.Id
                          join publisher in _context.Publishers
                          on product.PublisherId equals publisher.Id
                          where product.Id == id
                          select new
                          {
                              Product = product,
                              Author = author,
                              Publisher = publisher
                          }).SingleOrDefault();

            ViewData["Detail"] = detail;

            //Get owner name
            var ownerId = _context.Products
                .Where(p => p.Id == id)
                .Select(p => p.OwnerId)
                .FirstOrDefault();

            var ownerName = _context.Users.FirstOrDefault(u => u.Id == ownerId)?.Name;

            ViewData["OwnerName"] = ownerName;

            //get related book
            var genreId = _context.Products
                .Where(p=> p.Id == id)
                .Select(p=>p.GenreId)
                .SingleOrDefault();

            var relatedBook = _context.Products
                .Where(p => p.GenreId == genreId);

            ViewData["RelatedBook"] = relatedBook.ToList();

            return View("/Views/Home/ProductDetail.cshtml");
        }


        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> AddCart(int id)
        {
            //Get current ID user
            var user = await _userManager.GetUserAsync(User);
            var customerId = user.Id;


            //check product exist in cart
            //Get all product in cart
            var cart = (from c in _context.Carts
                        join product in _context.Products
                        on c.ProductId equals product.Id

                        where product.Id == id && c.CustomerId == customerId

                        select new
                        {
                            Cart = c,
                            CartId = c.Id,
                            Product = product
                        }).ToListAsync();


            if (cart.Result.Count == 0)
            {
                Cart cartObject = new Cart
                {
                    ProductId = id,
                    CustomerId = customerId,
                    Quantity = 1
                };

                _context.Add(cartObject);

            }
            else
            {
                var cartFirst = (from c in _context.Carts

                                 where c.ProductId == id && c.CustomerId == customerId

                                 select new
                                 {
                                     CartId = c.Id
                                 }).SingleOrDefault();
                //get cart id
                int cartId = cartFirst.CartId;
                //get cartObject by cart id
                var getCart = await _context.Carts.FindAsync(cartId);
                if (getCart != null)
                {
                    getCart.Quantity += 1;
                    _context.Update(getCart);
                }

            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }



        public async Task<IActionResult> UpdateQty(int id)
        {

            var getQty = Request.Query["inputQty"];
            //get cart by cart id
            var getCart = await _context.Carts.FindAsync(id);
            getCart.Quantity = int.Parse(getQty);

            _context.Update(getCart);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Cart));
        }

        public async Task<IActionResult> DeleteCart(int id)
        {
            /*var deleteCart = _context.Carts.Where(c => c.Id == id);*/
            var deleteCart = await _context.Carts.FindAsync(id);

            if (deleteCart != null)
            {
                _context.Carts.Remove(deleteCart);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Cart));
        }



        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Cart()
        {

            //Get current ID user
            var users = await _userManager.GetUserAsync(User);
            var customerId = users.Id;

            var cartDetail = (from cart in _context.Carts
                              join product in _context.Products
                              on cart.ProductId equals product.Id

                              join author in _context.Authorities
                              on product.AuthorId equals author.Id

                              where cart.CustomerId == customerId

                              select new
                              {
                                  Cart = cart,
                                  Product = product,
                                  Author = author
                              }).ToList();


            /*ViewData["cartDetail"] = cartDetail;*/

            return View("/Views/Home/Cart.cshtml", cartDetail);
        }

        public async Task<IActionResult> Checkout()
        {
            var users = await _userManager.GetUserAsync(User);
            var customerId = users.Id;

            var getCart = (from cart in _context.Carts

                           join product in _context.Products
                           on cart.ProductId equals product.Id

                           join owner in _context.Users
                           on product.OwnerId equals owner.Id

                           where cart.CustomerId == customerId

                           select new
                           {
                               Cart = cart,
                               Product = product,
                               Owner = owner

                           }).ToList();


            if (getCart.Count != 0)
            {
                var totalPrice = getCart.Sum(item => item.Product.Price * item.Cart.Quantity);



                //get current date
                var currentDate = DateTime.Now;
                string sqlFormatedDate = currentDate.ToString("yyyy-MM-dd");

                //get status
                var status = false;

                //Add data into order table
                Order orderObject = new Order
                {
                    CustomerId = customerId,
                    OrderDate = currentDate,
                    Status = status
                };

                _context.Orders.Add(orderObject);
                await _context.SaveChangesAsync();



                var getOrder = (from order in _context.Orders

                                where order.CustomerId == customerId
                                orderby order.Id
                                select new
                                {
                                    OrderId = order.Id,
                                    OrderStatus = order.Status
                                }).LastOrDefault();
                //get OrderId
                var getOrderId = getOrder.OrderId;

                //get status in Order
                var getStatus = getOrder.OrderStatus.ToString();

                //Add data into OrderDetail table
                foreach (var item in getCart)
                {
                    OrderDetail orderDetail = new OrderDetail
                    {
                        OrderId = getOrderId,
                        ProductId = item.Product.Id,
                        OwnerId = item.Owner.Id,
                        Quantity = item.Cart.Quantity,
                        Total = item.Product.Price * item.Cart.Quantity

                    };
                    _context.OrderDetails.Add(orderDetail);
                }

                await _context.SaveChangesAsync();

                //Delete all item in cart
                foreach (var item in getCart)
                {
                    var cartItem = _context.Carts.FindAsync(item.Cart.Id);
                    if (cartItem != null)
                    {
                        _context.Carts.Remove(await cartItem);
                    }
                }
                await _context.SaveChangesAsync();


                ViewData["Date"] = sqlFormatedDate;

                ViewData["Status"] = getStatus;
                ViewData["TotalPrice"] = totalPrice;
            }

            return View("/Views/Home/Checkout.cshtml", getCart);
        }


        public IActionResult DisplayProductByGenre(int id)
        {
            //var productList = await _context.Products.Include(p => p.Author).Where(p => p.GenreId == id).ToListAsync();
            var productList = (from product in _context.Products
                               join author in _context.Authorities
                               on product.AuthorId equals author.Id
                               where product.GenreId == id
                               select new
                               {
                                   Product = product,
                                   Author = author,
                               }).ToList();

            var genreName = _context.Genres.FirstOrDefault(u => u.Id == id)?.Name;

            ViewData["DisplayGenre"] = 1;
            ViewData["GenreName"] = genreName;

            return View("/Views/Home/DisplayProduct.cshtml", productList);
        }

        public IActionResult DisplayProductBySearch()
        {
            var searchTerm = Request.Query["search-term"];
            var productList = (from product in _context.Products
                               join author in _context.Authorities
                               on product.AuthorId equals author.Id
                               where product.Name.Contains(searchTerm)
                               select new
                               {
                                   Product = product,
                                   Author = author,
                               }).ToList();

            ViewData["SearchTerm"] = searchTerm;
            ViewData["DisplaySearch"] = 1;
            return View("/Views/Home/DisplayProduct.cshtml", productList);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index");
        }

        public IActionResult HelpCenter()
        {
            return View("Views/Home/help.cshtml");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}