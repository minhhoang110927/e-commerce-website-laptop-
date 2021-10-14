using Microsoft.AspNetCore.Mvc;
using SellingLaptop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SellingLaptop.Controllers
{
    public class ProductController : Controller
    {
        private readonly DataContext dataContext;
        public ProductController(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }
        public IActionResult Index()
        {
            ViewBag.Products = dataContext.Products.ToList();
            return View();
        }
        [HttpGet("Detail/{name}")]
        public IActionResult Detail(int id)
        {
            Product product = dataContext.Products.FirstOrDefault(p => p.ProductId == id);

            return View(product);
        }
    }
}
