using SunPerfume.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunPerfume.DataAccess.Repository.IRepository
{
    public interface ICartRepository : IRepository<Cart>
    {
        void IncrementCount(Cart cart, int count);
        void DecrementCount(Cart cart, int count);
    }
}
