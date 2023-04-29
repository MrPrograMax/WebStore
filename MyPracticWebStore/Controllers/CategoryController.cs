using Microsoft.AspNetCore.Mvc;
using MyPracticWebStore_DataAccess.Data;
using MyPracticWebStore_DataAccess.Repository.IRepository;
using MyPracticWebStore_Models;
using System.Collections.Generic;

namespace MyPracticWebStore.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public IActionResult Index()
        {
            IEnumerable<Category> itemList = _categoryRepository.GetAll();
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
                _categoryRepository.Add(item);
                _categoryRepository.Save();
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

            var item = _categoryRepository.Find(id.GetValueOrDefault());
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
                _categoryRepository.Update(item);
                _categoryRepository.Save();
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

            var item = _categoryRepository.Find(id.GetValueOrDefault());
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
            var item = _categoryRepository.Find(id.GetValueOrDefault());

            if (item == null)
            {
                return NotFound();
            }

            _categoryRepository.Remove(item);
            _categoryRepository.Save();
            return RedirectToAction("Index");
            

        }
    }
}
