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

namespace SellingLaptop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrator")]
    public class OrdersController : Controller
    {
        private readonly DataContext dataContext;
        private readonly SignInManager<User> signInManager;
        private readonly UserManager<User> userManager;

        public OrdersController(DataContext dataContext, SignInManager<User> signInManager, UserManager<User> userManager)
        {
            this.dataContext = dataContext;
            this.signInManager = signInManager;
            this.userManager = userManager;
        }
        public IActionResult Index()
        {
            ViewBag.OrdersList = dataContext.Orders.ToList();
            return View();
        }
        public IActionResult Detail(int id)
        {
            var ordersDetailList = dataContext.OrderDetails.Include(p => p.Product).Where(p => p.OrderId == id).ToList();
            ViewBag.OrdersDetailList = ordersDetailList;
            ViewBag.ThisOrders = dataContext.Orders.Find(id);
            return View();
        }
        public IActionResult DeleteOrders(int id)
        {
            var listOrdersDetail = dataContext.OrderDetails.Where(p => p.OrderId == id).ToList();
            foreach(var item in listOrdersDetail)
            {
                dataContext.OrderDetails.Remove(item);
            }
            var thisOrders = dataContext.Orders.FirstOrDefault(p => p.OrderId == id);
            dataContext.Orders.Remove(thisOrders);
            dataContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult ChangeStatus(int id)
        {
            var thisOrders = dataContext.Orders.FirstOrDefault(p => p.OrderId == id);
            var userEmail = dataContext.Users.FirstOrDefault(p => p.Id == thisOrders.CustomerId).Email;
            if (thisOrders.Status == Status.unconfimred)
            {
                thisOrders.Status = Status.confirmed;
            } else if (thisOrders.Status == Status.confirmed)
            {
                thisOrders.Status = Status.delivering;
            }
            else if (thisOrders.Status == Status.delivering)
            {
                thisOrders.Status = Status.delivered;
            }
            else
            {
                return NotFound();
            }
            string subject = "Cap nhat trang thai don hang";
            string content = "Don hang cua ban da chuyen sang trang thai " + "'" + thisOrders.Status + "'";
            new MailHelper().SendMail("sellinglaptop0@gmail.com", "0982994297", userEmail, subject, content);

            dataContext.Entry(thisOrders).State = EntityState.Modified;
            dataContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
