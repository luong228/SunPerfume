using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunPerfume.Models.ViewModels
{
    public class HomeVM
    {
        public IEnumerable<Category> CategoryList { get; set; }
        public IEnumerable<Product> ProductList { get; set; }

    }
}
