using Microsoft.AspNetCore.Mvc;
using SellingLaptop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SellingLaptop.Controllers
{
    public class CategoryController : Controller
    {
        private readonly DataContext dataContext;
        public CategoryController(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }
        public IActionResult Index()
        {
            ViewBag.Categories = dataContext.Categories.ToList();
            return View();
        }
        [HttpGet("ByCategory/{name}")]
        public IActionResult ByCategory(int id)
        {
            ViewBag.CategoryName = dataContext.Categories.FirstOrDefault(p => p.CategoryId == id).CategoryName;
            ViewBag.ByCategory = dataContext.Products.Where(p => p.CategoryId == id).ToList();
            return View();
        }
    }
}
