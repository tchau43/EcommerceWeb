using Microsoft.AspNetCore.Mvc;
using WebBanHang.Data;
using WebBanHang.ViewModels;

namespace WebBanHang.ViewComponents
{
    public class MenuLoaiViewComponent : ViewComponent
    {
        private readonly Hshop2023Context db;

        public MenuLoaiViewComponent(Hshop2023Context context) => db = context;

        public IViewComponentResult Invoke()
        {
            var data = db.Loais.Select(loai => new MenuLoaiVM
            {
                maLoai = loai.MaLoai,
                tenLoai = loai.TenLoai,
                soLuong = loai.HangHoas.Count
            }).OrderBy(p => p.tenLoai);
            return View(data);
        }
    }
}
