using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunPerfume.Models.ViewModels
{
    public class ProductVM
    {
        //Customer Area
        [ValidateNever]
        public IEnumerable<Product> ProductList { get; set; }
        [ValidateNever]
        public Category Category { get; set; }
        [ValidateNever]
        public Brand Brand { get; set; }
        // Admin Area
        public Product Product { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> CategorySelectList { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> BrandSelectList { get; set; }

    }
}
