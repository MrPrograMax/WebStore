using MyPracticWebStore_DataAccess.Data;
using MyPracticWebStore_DataAccess.Repository.IRepository;
using MyPracticWebStore_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPracticWebStore_DataAccess.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _db;
        public CategoryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(Category item)
        {
            var itemFromDb = base.FirstOrDefault(u => u.Id == item.Id);

            if (itemFromDb != null)
            {
                itemFromDb.Name = item.Name;
                itemFromDb.DisplayOrder = item.DisplayOrder;
            }
        }
    }
}
