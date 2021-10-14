using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SellingLaptop.Models
{
    public class Category
    {
        [Key]

        [Required(ErrorMessage = "Chọn danh mục")]
        public int CategoryId { get; set; }

        [DisplayName("CategoryName")]
        [Required(ErrorMessage = "Nhập tên danh mục")]
        public string CategoryName { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
