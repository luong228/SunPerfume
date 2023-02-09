using SunPerfume.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunPerfume.Models.ViewModels
{
    public class OrderVM
    {
        // Order Management in Admin section - Like CartVM for Customer
        public OrderHeader OrderHeader { get; set; }
        public IEnumerable<OrderDetail> OrderDetail { get; set; }
    }
}
