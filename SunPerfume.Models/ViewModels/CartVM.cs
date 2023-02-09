using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunPerfume.Models.ViewModels
{
    public class CartVM
    {
       public IEnumerable<Cart> ListCart { get; set; }
       public OrderHeader OrderHeader { get; set; }

    }
}
