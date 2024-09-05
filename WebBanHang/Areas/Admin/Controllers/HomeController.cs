using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebBanHang.Data;
using WebBanHang.ViewModels;

namespace WebBanHang.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Route("Admin")]
    [Route("Admin/Home")]
    public class HomeController : Controller
    {
        private readonly Hshop2023Context db;

        public HomeController(Hshop2023Context context)
        {
            db = context;
        }
        [Route("")]
        [Route("index")]
        public IActionResult Index()
        {
            var hangHoas = db.HangHoas.AsQueryable();

            var result = hangHoas.Select(p => new HangHoaVM
            {
                MaHang = p.MaHh,
                TenHang = p.TenHh,
                DonGiaHang = p.DonGia ?? 0,
                HinhHang = p.Hinh ?? "",
                MoTaNganHang = p.MoTaDonVi ?? "",
                TenLoaiHang = p.MaLoaiNavigation.TenLoai ?? ""
            });

            // Assuming the name is stored as a claim of type ClaimTypes.Name
            var adminName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            // Pass the name to the view using ViewBag
            ViewBag.AdminName = adminName;

            return View(result);
        }
    }
}
