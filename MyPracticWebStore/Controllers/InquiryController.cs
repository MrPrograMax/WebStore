using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyPracticWebStore_DataAccess.Repository.IRepository;
using MyPracticWebStore_Models;
using MyPracticWebStore_Utility;
using System.Collections.Generic;
using WebPracticWebStore_Models;
using WebPracticWebStore_Models.ViewModels;

namespace MyPracticWebStore.Controllers
{
    [Authorize(Roles = WebConstants.AdminRole)]
    public class InquiryController : Controller
    {
        public readonly IInquiryHeaderRepository _inquiryHeaderRepository;
        public readonly IInquiryDetailRepository _inquiryDetailRepository;

        [BindProperty]
        public InquiryVM InquiryVM { get; set; }

        public InquiryController(IInquiryHeaderRepository inquiryHeaderRepository, 
            IInquiryDetailRepository inquiryDetailRepository)
        {
            _inquiryHeaderRepository = inquiryHeaderRepository;
            _inquiryDetailRepository = inquiryDetailRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            InquiryVM = new InquiryVM()
            {
                InquiryHeader = _inquiryHeaderRepository.FirstOrDefault(u => u.Id == id),
                InquiryDetail = _inquiryDetailRepository.GetAll(u => u.InquiryHeaderId == id, includePropirties: "Product")
            };
            return View(InquiryVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Details()
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>(); 
            InquiryVM.InquiryDetail = _inquiryDetailRepository.GetAll(u => u.InquiryHeaderId == InquiryVM.InquiryHeader.Id);

            foreach (var detail in InquiryVM.InquiryDetail)
            {
                ShoppingCart shoppingCart = new ShoppingCart()
                {
                    ProductId = detail.ProductId,
                    Count = 1,
                };
                shoppingCartList.Add(shoppingCart);
            }

            HttpContext.Session.Clear();
            HttpContext.Session.Set(WebConstants.SessionCart, shoppingCartList);
            HttpContext.Session.Set(WebConstants.SessionInquiryId, InquiryVM.InquiryHeader.Id);


            return RedirectToAction("Index", "Cart"); 
        }


        [HttpPost]
        public IActionResult Delete()
        {
            InquiryHeader inquiryHeader = 
                _inquiryHeaderRepository.FirstOrDefault(u => u.Id == InquiryVM.InquiryHeader.Id);

            IEnumerable<InquiryDetail> inquiryDetails = 
                _inquiryDetailRepository.GetAll(u => u.InquiryHeaderId == InquiryVM.InquiryHeader.Id);

            _inquiryDetailRepository.RemoveRange(inquiryDetails);
            _inquiryHeaderRepository.Remove(inquiryHeader);
            _inquiryDetailRepository.Save();

            TempData[WebConstants.Success] = "Action completed successfully";

            return RedirectToAction(nameof(Index));

        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetInquiryList() 
        {
            return Json( new { data = _inquiryHeaderRepository.GetAll() });
        }

        #endregion
    }
}
