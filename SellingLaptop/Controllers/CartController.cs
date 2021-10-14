using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SellingLaptop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SellingLaptop.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly DataContext dataContext;
        private readonly SignInManager<User> signInManager;
        private readonly UserManager<User> userManager;
        public CartController(DataContext dataContext, SignInManager<User> signInManager, UserManager<User> userManager)
        {
            this.dataContext = dataContext;
            this.signInManager = signInManager;
            this.userManager = userManager;
        }
        public IActionResult Index()
        {
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cartid = dataContext.Users.FirstOrDefault(p => p.Id == userid).CartId;
            var cart = dataContext.Carts.Find(cartid);
            var listCartDetail = dataContext.CartDetails.Include(p => p.Product).Where(p => p.CartId == cartid).ToList();
            ViewBag.listCartDetail = listCartDetail;
            ViewBag.Total = cart.Total;
            return View();
        }
        public IActionResult AddToCart(int id)
        {
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cartid = dataContext.Users.FirstOrDefault(p => p.Id == userid).CartId;
            var cart = dataContext.Carts.Find(cartid);
            CartDetail cartdetail = new CartDetail() { CartId = cartid, ProductId = id };
            var product = dataContext.Products.FirstOrDefault(p => p.ProductId == id);
            cart.Total += product.ProductPrice;
            dataContext.Add(cartdetail);
            dataContext.Entry(cart).State = EntityState.Modified;
            dataContext.SaveChanges();
            return RedirectToAction("Index", "Cart");
        }
        public IActionResult RemoveItem(int id)
        {
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cartid = dataContext.Users.FirstOrDefault(p => p.Id == userid).CartId;
            var cart = dataContext.Carts.Find(cartid);
            var cartdetail = dataContext.CartDetails.Include(p => p.Product).Where(p => p.CartDetailId == id).FirstOrDefault();

            dataContext.CartDetails.Remove(cartdetail);
            cart.Total -= cartdetail.Product.ProductPrice;
            dataContext.Entry(cart).State = EntityState.Modified;
            dataContext.SaveChanges();
            return RedirectToAction("Index", "Cart");
        }
        public IActionResult Order()
        {
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = dataContext.Users.Find(userid);
            ViewBag.CustomerName = user.FullName;
            ViewBag.CustomerId = userid;
            var cartid = dataContext.Users.FirstOrDefault(p => p.Id == userid).CartId;
            var cart = dataContext.Carts.Find(cartid);
            ViewBag.Total = cart.Total;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Order(Order order)
        {
            if (!ModelState.IsValid)
            {
                return View(order);
            }
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userEmail = dataContext.Users.Find(userid).Email;
            var cartid = dataContext.Users.FirstOrDefault(p => p.Id == userid).CartId;
            var cart = dataContext.Carts.Find(cartid);
            var listCartDetail = dataContext.CartDetails.Include(p => p.Product).Where(p => p.CartId == cartid).ToList();

            Order newOrder = order;
            newOrder.CustomerId = userid;
            newOrder.CustomerName = dataContext.Users.Find(userid).FullName;
            newOrder.OrderDate = DateTime.Now;
            newOrder.Total = cart.Total;
            newOrder.Status = Status.unconfimred;
            dataContext.Orders.Add(newOrder);
            await dataContext.SaveChangesAsync();

            foreach (var item in listCartDetail)
            {
                OrderDetails orderDetails = new OrderDetails();
                orderDetails.OrderId = newOrder.OrderId;
                orderDetails.ProductId = item.ProductId;
                dataContext.OrderDetails.Add(orderDetails);
                //RemoveItem(item.CartDetailId);
                var cartDetail = await dataContext.CartDetails.FindAsync(item.CartDetailId);
                dataContext.CartDetails.Remove(cartDetail);
            }
            cart.Total = 0;
            dataContext.Entry(cart).State = EntityState.Modified;
            await dataContext.SaveChangesAsync();
            string content = "Ban da dat hang thanh cong";
            new MailHelper().SendMail("sellinglaptop0@gmail.com", "0982994297", userEmail, "Dat hang thanh cong", content);
            return RedirectToAction("MyOrders", "Cart");
        }
        public async Task<IActionResult> MyOrders()
        {
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewBag.OrdersList = await dataContext.Orders.Where(p => p.CustomerId == userid).ToListAsync();
            return View();
        }
        public IActionResult OrdersDetail(int id)
        {
            var ordersDetailList = dataContext.OrderDetails.Include(p => p.Product).Where(p => p.OrderId == id).ToList();
            ViewBag.OrdersDetailList = ordersDetailList;
            ViewBag.ThisOrders = dataContext.Orders.Find(id);
            return View();
        }
        [HttpGet]
        public async Task<String> GetCurrentUserId()
        {
            User user = await userManager.GetUserAsync(HttpContext.User);
            return user.Id;
        }
    }
}
