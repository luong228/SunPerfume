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
    public class BrandRepository : Repository<Brand>, IBrandRepository
    {
        private readonly ApplicationDbContext _db;
        public BrandRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Brand obj)
        {
            _db.Brands.Update(obj);
        }
    }
}
