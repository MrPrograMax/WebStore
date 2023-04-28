using Microsoft.AspNetCore.Authorization;
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

namespace MyPracticWebStore.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly EmailService _emailService;



        [BindProperty]
        public ProductUserVM ProductUserVM { get; set; }

        public CartController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment, EmailService emailService)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
            _emailService = 
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
            IEnumerable<Product> productList = _db.Product.Where(u => prodInCart.Contains(u.Id));


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
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            List<ShoppingCart> shoppingCartsList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart).Count() > 0)
            {
                //session exsits
                shoppingCartsList = HttpContext.Session.Get<List<ShoppingCart>>(WebConstants.SessionCart);
            }

            List<int> prodInCart = shoppingCartsList.Select(i => i.ProductId).ToList();
            IEnumerable<Product> productList = _db.Product.Where(u => prodInCart.Contains(u.Id));

            ProductUserVM = new ProductUserVM()
            {
                ApplicationUser = _db.ApplicationUser.FirstOrDefault(u => u.Id == claim.Value),
                ProductList = productList.ToList()
            };

            return View(ProductUserVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPost(ProductUserVM productUserVM)
        {
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
            foreach (var prod in productUserVM.ProductList)
            {
                productListSB.Append($" - Name: {prod.Name} <span style='font-size: 14px'> (ID: {prod.Id})</span><br />");
            }

            string messageBody = string.Format(HtmlBody, 
                ProductUserVM.ApplicationUser.FullName,
                ProductUserVM.ApplicationUser.Email,
                ProductUserVM.ApplicationUser.PhoneNumber,
                productListSB.ToString());

            await _emailService.SendEmailAsync("WebStoreGet@yandex.ru", "New order", messageBody);

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


    }
}
