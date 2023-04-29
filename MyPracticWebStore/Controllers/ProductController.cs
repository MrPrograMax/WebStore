using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyPracticWebStore_DataAccess.Data;
using MyPracticWebStore_DataAccess.Repository.IRepository;
using MyPracticWebStore_Models;
using MyPracticWebStore_Models.ViewModels;
using MyPracticWebStore_Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MyPracticWebStore.Controllers
{
    [Authorize(Roles = WebConstants.AdminRole)]
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IProductRepository productRepository, IWebHostEnvironment webHostEnvironment)
        {
            _productRepository = productRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> itemList = _productRepository.GetAll(includePropirties: "Category");

            return View(itemList); 
        }

        //GET - Create and Update
        public IActionResult Upsert(int? id)
        {

            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategorySelectList = _productRepository.GetAllDropdownList(WebConstants.CategoryName)
            };

            if (id == null)
            {
                return View(productVM);
            }
            else
            {
                productVM.Product = _productRepository.Find(id.GetValueOrDefault());

                if (productVM.Product == null)
                {
                    return NotFound();
                }

                return View(productVM);
            }
        }

        //POST - Create and Update
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productVM)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                string webRootPath = _webHostEnvironment.WebRootPath;

                if (productVM.Product.Id == 0)
                {
                    //Creating
                    string upload = webRootPath + WebConstants.ImagePath;
                    string fileName = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(files[0].FileName);

                    using (var fileStream = new FileStream(Path.Combine(upload,  fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }
                     
                    productVM.Product.Image = fileName + extension;

                    _productRepository.Add(productVM.Product);
                }
                else
                {
                    //Updating
                    var itemFromDb = _productRepository.FirstOrDefault(u => u.Id == productVM.Product.Id, isTracking: false);
                    
                    if (files.Count > 0) //New file already received
                    {
                        string upload = webRootPath + WebConstants.ImagePath;
                        string fileName = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(files[0].FileName);

                        var oldFile = Path.Combine(upload, itemFromDb.Image);

                        if (System.IO.File.Exists(oldFile))
                        {
                            System.IO.File.Delete(oldFile); 
                        }

                        using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);
                        }

                        productVM.Product.Image = fileName + extension;
                    }
                    else //We didn't get new photo but other fields changed
                    {
                        productVM.Product.Image = itemFromDb.Image;
                    }

                    _productRepository.Update(productVM.Product);
                }

                _productRepository.Save();

                TempData[WebConstants.Success] = "Action completed successfully";

                return RedirectToAction("Index");
            }

            productVM.CategorySelectList = _productRepository.GetAllDropdownList(WebConstants.CategoryName);

            return View(productVM); 
        }



        //GET - DELETE
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Product product = _productRepository.FirstOrDefault(u => u.Id == id, includePropirties: "Category");

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        //POST - DELETE
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var item = _productRepository.Find(id.GetValueOrDefault());

            if (item == null)
            {
                return NotFound();
            }

            string upload = _webHostEnvironment.WebRootPath + WebConstants.ImagePath;
            var oldFile = Path.Combine(upload, item.Image);

            if (System.IO.File.Exists(oldFile))
            {
                System.IO.File.Delete(oldFile);
            }

            _productRepository.Remove(item);
            _productRepository.Save();

            TempData[WebConstants.Success] = "Action completed successfully";

            return RedirectToAction("Index");
            

        }
    }
}
