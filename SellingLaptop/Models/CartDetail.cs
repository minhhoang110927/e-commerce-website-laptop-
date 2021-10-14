using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SellingLaptop.Models
{
    public class CartDetail
    {
        [Key]
        public int CartDetailId { get; set; }
        public int CartId { get; set; }
        public virtual Product Product { get; set; }
        public int ProductId { get; set; }


    }
}
