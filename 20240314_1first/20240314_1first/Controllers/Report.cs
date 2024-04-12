using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _20240314_1first.Controllers
{
    public enum ReportMode
    {
        Brand = 0,
        Product = 1,
        Category = 2,
        All = 3
    }
    public partial class Report
    {
       
        public ReportMode Mode { get; set; }
        public string CustomerName { get; set; }
        public IEnumerable<ReportDetail> ReportDetails { get; set; }
       
       
    }
}
