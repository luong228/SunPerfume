using SunPerfume.DataAccess.Data;
using SunPerfume.DataAccess.Repository.IRepository;
using SunPerfume.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunPerfume.DataAccess.Repository
{
    public class CartRepository : Repository<Cart>, ICartRepository
    {
        private readonly ApplicationDbContext _db;
        public CartRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void DecrementCount(Cart cart, int count)
        {
            throw new NotImplementedException();
        }

        public void IncrementCount(Cart cart, int count)
        {
            throw new NotImplementedException();
        }
    }
}
