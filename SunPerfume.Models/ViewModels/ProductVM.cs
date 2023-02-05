using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunPerfume.Models.ViewModels
{
    public class ProductVM
    {
        public IEnumerable<Product> Product { get; set; }
        public Category Category { get; set; }
        public Brand Brand { get; set; }

    }
}
