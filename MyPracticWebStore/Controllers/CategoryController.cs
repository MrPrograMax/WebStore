using Microsoft.AspNetCore.Mvc;
using MyPracticWebStore.Data;
using MyPracticWebStore.Models;
using System.Collections.Generic;

namespace MyPracticWebStore.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            IEnumerable<Category> itemList = _db.Category;
            return View(itemList);
        }

        //GET - CREATE
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category item)
        {
            _db.Category.Add(item);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
