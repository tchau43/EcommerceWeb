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
        private readonly Hshop2023Context db; // Giả sử bạn đã có DbContext tên là Hshop2023Context

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

            // Tổng doanh thu toàn thời gian
            model.TotalRevenue = db.HoaDons
                                   .Where(hd => hd.ThanhTien.HasValue)
                                   .Sum(hd => hd.ThanhTien.Value);

            // Doanh thu theo khoảng thời gian được chọn, nếu có
            if (startDate.HasValue && endDate.HasValue)
            {
                model.SelectedPeriodRevenue = db.HoaDons
                                                .Where(hd => hd.ThanhTien.HasValue &&
                                                             hd.NgayDat >= startDate.Value &&
                                                             hd.NgayDat <= endDate.Value)
                                                .Sum(hd => hd.ThanhTien.Value);
                model.StartDate = startDate.Value;
                model.EndDate = endDate.Value;
            }

            return View(model);
        }

    }
}
