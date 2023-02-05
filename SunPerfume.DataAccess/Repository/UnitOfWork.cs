using SunPerfume.DataAccess.Data;
using SunPerfume.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunPerfume.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            CategoryRepository = new CategoryRepository(_db);
            BrandRepository = new BrandRepository(_db);
            ProductRepository = new ProductRepository(_db);
            CartRepository = new CartRepository(_db);

        }

        public ICategoryRepository CategoryRepository { get; private set; }

        public IBrandRepository BrandRepository { get; private set; }

        public IProductRepository ProductRepository { get; private set; }

        public ICartRepository CartRepository { get; private set; }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
