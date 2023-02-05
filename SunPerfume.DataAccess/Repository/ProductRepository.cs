﻿using SunPerfume.DataAccess.Data;
using SunPerfume.DataAccess.Repository.IRepository;
using SunPerfume.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunPerfume.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db): base(db)
        { 
            _db = db;
        }

        public void Update(Product obj)
        {
            throw new NotImplementedException();
        }
    }
}
