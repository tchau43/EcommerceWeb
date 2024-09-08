using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Areas.Admin.ViewModels;
using WebBanHang.Data;

namespace WebBanHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin")]
    [Route("Statistics")]
    public class StatisticsController : Controller
    {
        private readonly Hshop2023Context db; 

        public StatisticsController(Hshop2023Context context)
        {
            db = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [Route("RevenueReport")]
        [HttpGet("RevenueReport")]
        public IActionResult RevenueReport(DateTime? startDate, DateTime? endDate)
        {
            var model = new RevenueVM();

            // Total revenue
            model.TotalRevenue = db.HoaDons
                                   .Where(hd => hd.ThanhTien.HasValue)
                                   .Sum(hd => hd.ThanhTien.Value);

            // Revenue for the selected period
            if (startDate.HasValue && endDate.HasValue)
            {
                var selectedRevenues = db.HoaDons
                                         .Where(hd => hd.ThanhTien.HasValue &&
                                                      hd.NgayDat >= startDate.Value &&
                                                      hd.NgayDat <= endDate.Value)
                                         .GroupBy(hd => hd.NgayDat.Date)
                                         .Select(g => new
                                         {
                                             Date = g.Key,
                                             Revenue = g.Sum(hd => hd.ThanhTien.Value)
                                         })
                                         .OrderBy(g => g.Date)
                                         .ToList();

                model.SelectedPeriodRevenue = selectedRevenues.Sum(x => x.Revenue);
                model.StartDate = startDate.Value;
                model.EndDate = endDate.Value;

                // Populate Labels and RevenueData
                model.Labels = selectedRevenues.Select(x => x.Date.ToShortDateString()).ToList();
                model.RevenueData = selectedRevenues.Select(x => x.Revenue).ToList();
            }
            return View(model);
        }
    }
}
