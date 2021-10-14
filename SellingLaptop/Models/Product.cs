using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SellingLaptop.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public string Processor { get; set; }
        public string Screen { get; set; }
        public string Ram { get; set; }
        public string Rom { get; set; }
        public double ProductPrice { get; set; }
        public virtual Category Category { get; set; }
        public int CategoryId { get; set; }

    }
}
