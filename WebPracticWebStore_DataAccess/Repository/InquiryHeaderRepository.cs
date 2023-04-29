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
    public class InquiryHeaderRepository : Repository<InquiryHeader>, IInquiryHeaderRepository
    {
        private readonly ApplicationDbContext _db;
        public InquiryHeaderRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(InquiryHeader item)
        {
           _db.InquiryHeader.Update(item);
        }
    }
}
