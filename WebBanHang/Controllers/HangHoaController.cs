using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Data;
using WebBanHang.ViewModels;

namespace WebBanHang.Controllers
{
    public class HangHoaController : Controller
    {
        private readonly Hshop2023Context db;

        public HangHoaController(Hshop2023Context context)
        {
            db = context;
        }

        public IActionResult Index(int? loai)
        {
            var hangHoas = db.HangHoas.AsQueryable();

            if (loai.HasValue)
            {
                hangHoas = hangHoas.Where(p => p.MaLoai == loai.Value);
            }

            var result = hangHoas.Select(p => new HangHoaVM
            {
                MaHang = p.MaHh,
                TenHang = p.TenHh,
                DonGiaHang = p.DonGia ?? 0,
                HinhHang = p.Hinh ?? "",
                MoTaNganHang = p.MoTaDonVi ?? "",
                TenLoaiHang = p.MaLoaiNavigation.TenLoai ?? ""
            });

            return View(result);
        }

        public IActionResult Search(string? query)
        {
            var hangHoas = db.HangHoas.AsQueryable();

            if (query != null)
            {
                hangHoas = hangHoas.Where(p => p.TenHh.Contains(query));
            }

            var result = hangHoas.Select(p => new HangHoaVM
            {
                MaHang = p.MaHh,
                TenHang = p.TenHh,
                DonGiaHang = p.DonGia ?? 0,
                HinhHang = p.Hinh ?? "",
                MoTaNganHang = p.MoTaDonVi ?? "",
                TenLoaiHang = p.MaLoaiNavigation.TenLoai ?? ""
            });

            return View(result);
        }

        public IActionResult Detail(int id)
        {
            var data = db.HangHoas
                .Include(p => p.MaLoaiNavigation)
                .SingleOrDefault(p => p.MaHh == id);
            if(data == null)
            {
                TempData["Message"] = $"Khong Thay Hang Co Ma {id} Can Tim";
                return Redirect("/404");
            }
            var result = new ChiTietHangHoaVM
            {
                MaHang = data.MaHh,
                TenHang = data.TenHh,
                DonGiaHang= data.DonGia ?? 0,
                ChiTietHang = data.MoTa ?? "",
                HinhHang= data.Hinh ?? "",
                MoTaNganHang= data.MoTaDonVi ?? "",
                TenLoaiHang= data.MaLoaiNavigation.TenLoai,
                SoLuongTon = 10,
                DiemDanhGiaHang = 5,
            };
            return View(result);
        }
    }
}
