using Microsoft.AspNetCore.Mvc.Rendering;
using MyPracticWebStore_DataAccess.Data;
using MyPracticWebStore_DataAccess.Repository.IRepository;
using MyPracticWebStore_Models;
using System.Collections.Generic;
using MyPracticWebStore_Utility;
using System.Linq;
using WebPracticWebStore_Models;

namespace MyPracticWebStore_DataAccess.Repository
{
    public class InquiryDetailRepository : Repository<InquiryDetail>, IInquiryDetailRepository
    {
        private readonly ApplicationDbContext _db;
        public InquiryDetailRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(InquiryDetail item)
        {
           _db.InquiryDetail.Update(item);
        }
    }
}
