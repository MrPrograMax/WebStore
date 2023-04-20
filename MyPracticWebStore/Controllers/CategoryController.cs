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
            if (ModelState.IsValid)
            {
                _db.Category.Add(item);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(item); 
        }

        //GET - EDIT
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var item = _db.Category.Find(id);
            if (item == null)
            {
                return NotFound();
            }



            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category item)
        {
            if (ModelState.IsValid)
            {
                _db.Category.Update(item);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(item);
        }


        //GET - DELETE
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var item = _db.Category.Find(id);
            if (item == null)
            {
                return NotFound();
            }



            return View(item);
        }

        //POST - DELETE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var item = _db.Category.Find(id);

            if (item == null)
            {
                return NotFound();
            }

            _db.Category.Remove(item);
            _db.SaveChanges();
            return RedirectToAction("Index");
            

        }
    }
}
