using Microsoft.AspNetCore.Mvc.Rendering;
using MyPracticWebStore_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPracticWebStore_DataAccess.Repository.IRepository
{
    public interface IProductRepository : IRepository<Product>
    {
        void Update(Product item);

        IEnumerable<SelectListItem> GetAllDropdownList(string item);
    }
}
