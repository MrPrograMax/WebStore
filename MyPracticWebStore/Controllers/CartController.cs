﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using MyPracticWebStore_DataAccess.Data;
using MyPracticWebStore_Models;
using MyPracticWebStore_Models.ViewModels;
using MyPracticWebStore_Utility;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System.Threading.Tasks;
using MyPracticWebStore_DataAccess.Repository.IRepository;
using MyPracticWebStore_DataAccess.Repository;
using WebPracticWebStore_Models;
using System;
using System.Drawing;

namespace MyPracticWebStore.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly IApplicationUserRepository _applicationUserRepository;
        private readonly IInquiryHeaderRepository _inquiryHeaderRepository;
        private readonly IInquiryDetailRepository _inquiryDetailRepository;

        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly EmailService _emailService;



        [BindProperty]
        public ProductUserVM ProductUserVM { get; set; }

        public CartController(IProductRepository productRepository, IApplicationUserRepository applicationUserRepository,
            IInquiryHeaderRepository inquiryHeaderRepository, IInquiryDetailRepository inquiryDetailRepository,
            IWebHostEnvironment webHostEnvironment, EmailService emailService)
        {
            _productRepository = productRepository;
            _applicationUserRepository = applicationUserRepository;
            _inquiryHeaderRepository = inquiryHeaderRepository;
            _inquiryDetailRepository = inquiryDetailRepository;
            _webHostEnvironment = webHostEnvironment;
            _emailService = emailService;
        }
        public IActionResult Index()
        {
            List<ShoppingCart> shoppingCartsList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart).Count() > 0)
            {
                //session exsits
                shoppingCartsList = HttpContext.Session.Get<List<ShoppingCart>>(WebConstants.SessionCart);
            }

            List<int> prodInCart = shoppingCartsList.Select(i => i.ProductId).ToList();
            IEnumerable<Product> productListTemp = _productRepository.GetAll(u => prodInCart.Contains(u.Id));
            IList<Product> productList = new List<Product>();

            foreach (var item in shoppingCartsList)
            {
                Product prodTemp = productListTemp.FirstOrDefault(u => u.Id == item.ProductId);
                prodTemp.TempCount = item.Count;
                productList.Add(prodTemp);
            }

            return View(productList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost()
        {
            return RedirectToAction(nameof(Summary));
        }

        public IActionResult Summary()
        {
            ApplicationUser applicationUser;


            if (HttpContext.Session.Get<int>(WebConstants.SessionInquiryId) != 0)
            {
                //cart has been loaded using an inquiry
                InquiryHeader inquiryHeader = _inquiryHeaderRepository.FirstOrDefault(u => u.Id == HttpContext.Session.Get<int>(WebConstants.SessionInquiryId));
                applicationUser = new ApplicationUser()
                {
                    Email = inquiryHeader.Email,
                    FullName = inquiryHeader.FullName,
                    PhoneNumber = inquiryHeader.PhoneNumber
                };
            }
            else
            {
                applicationUser = new ApplicationUser();
            }


            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            //var userId = User.FindFirstValue(ClaimTypes.Name);

            applicationUser = _applicationUserRepository.FirstOrDefault(u => u.Id == claim.Value);



            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart).Count() > 0)
            {
                //session exsits
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WebConstants.SessionCart);
            }
            List<int> prodInCart = shoppingCartList.Select(i => i.ProductId).ToList();
            IEnumerable<Product> prodList = _productRepository.GetAll(u => prodInCart.Contains(u.Id));

            ProductUserVM = new ProductUserVM()
            {
                ApplicationUser = applicationUser,
            };

            foreach (var item in shoppingCartList)
            {
                Product prodTemp = _productRepository.FirstOrDefault(u => u.Id == item.ProductId);
                prodTemp.TempCount = item.Count;
                ProductUserVM.ProductList.Add(prodTemp);
            }

            return View(ProductUserVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPost(ProductUserVM productUserVM)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var PathToTemplate = _webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar.ToString()
                + "templates" + Path.DirectorySeparatorChar.ToString()
                + "inquiry.html";

            var subject = "New inquiry";
            string HtmlBody = "";

            using (StreamReader sr = System.IO.File.OpenText(PathToTemplate))
            {
                HtmlBody = sr.ReadToEnd();
            }

            //inquiry.html has fields
            //Name: {0}
            //Email: {1}
            //Phone: {2}
            //Products: {3}

            StringBuilder productListSB = new StringBuilder();

            var orderTotalPrice = 0.0;

            foreach (var prod in productUserVM.ProductList)
            {
                productListSB.Append($" - Name: {prod.Name}, Count: {prod.TempCount} PriceForThis: {prod.TempCount * prod.Price}<span style='font-size: 14px'> (ID: {prod.Id})</span><br />");
                orderTotalPrice += prod.TempCount * prod.Price;
            }
            productListSB.Append($"<br />Total Price: {orderTotalPrice.ToString("c")}");

            string messageBody = string.Format(HtmlBody,
                ProductUserVM.ApplicationUser.FullName,
                ProductUserVM.ApplicationUser.Email,
                ProductUserVM.ApplicationUser.PhoneNumber,
                productListSB.ToString());

            await _emailService.SendEmailAsync(WebConstants.emailOrder, "New order", messageBody);

            InquiryHeader inquiryHeader = new InquiryHeader()
            {
                ApplicationUserId = claim.Value,
                FullName = productUserVM.ApplicationUser.FullName,
                Email = productUserVM.ApplicationUser.Email,
                PhoneNumber = productUserVM.ApplicationUser.PhoneNumber,
                InquiryDate = DateTime.Now
            };

            _inquiryHeaderRepository.Add(inquiryHeader);
            _inquiryHeaderRepository.Save();

            foreach (var prod in productUserVM.ProductList)
            {
                InquiryDetail inquiryDetail = new InquiryDetail()
                {
                    InquiryHeaderId = inquiryHeader.Id,
                    ProductId = prod.Id,
                    Count = prod.TempCount
                };
                _inquiryDetailRepository.Add(inquiryDetail);
            }

            _inquiryDetailRepository.Save();

            TempData[WebConstants.Success] = "Inquiry submitted successfully";

            return RedirectToAction(nameof(InquiryConfirmation));
        }

        public IActionResult InquiryConfirmation()
        {
            HttpContext.Session.Clear();
            return View();
        }
        public IActionResult Remove(int id)
        {
            List<ShoppingCart> shoppingCartsList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart).Count() > 0)
            {
                //session exsits
                shoppingCartsList = HttpContext.Session.Get<List<ShoppingCart>>(WebConstants.SessionCart);
            }

            shoppingCartsList.Remove(shoppingCartsList.FirstOrDefault(u => u.ProductId == id));

            HttpContext.Session.Set(WebConstants.SessionCart, shoppingCartsList);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateCart(IEnumerable<Product> ProdList)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            foreach (Product prod in ProdList)
            {
                shoppingCartList.Add(new ShoppingCart { ProductId = prod.Id, Count = prod.TempCount });
            }

            HttpContext.Session.Set(WebConstants.SessionCart, shoppingCartList);
            return RedirectToAction(nameof(Index));
        }

    }
}
