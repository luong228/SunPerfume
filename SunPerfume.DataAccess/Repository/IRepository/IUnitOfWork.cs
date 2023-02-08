using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunPerfume.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository CategoryRepository { get; } 
        IBrandRepository BrandRepository { get; }
        IProductRepository ProductRepository { get; }
        ICartRepository CartRepository { get; }
        ICompanyRepository CompanyRepository { get; }
        void Save();
    }
}
