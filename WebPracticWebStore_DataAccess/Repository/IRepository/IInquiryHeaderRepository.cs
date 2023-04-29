using Microsoft.AspNetCore.Mvc.Rendering;
using MyPracticWebStore_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebPracticWebStore_Models;

namespace MyPracticWebStore_DataAccess.Repository.IRepository
{
    public interface IInquiryHeaderRepository : IRepository<InquiryHeader>
    {
        void Update(InquiryHeader item);
    }
}
