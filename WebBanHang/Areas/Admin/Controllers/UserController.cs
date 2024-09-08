using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Areas.Admin.ViewModels;
using WebBanHang.Data;
using WebBanHang.Helpers;
using WebBanHang.ViewModels;

namespace WebBanHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin")]
    [Route("User")]
    public class UserController : Controller
    {
        private readonly Hshop2023Context db;

        public UserController(Hshop2023Context context)
        {
            db = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("UserControl")]
        [HttpGet]
        public IActionResult UserControl()
        {
            var _user = db.KhachHangs.AsQueryable();
            var model = _user.Select(p => new UserVM
            {
                MaKh = p.MaKh,
                MatKhau = p.MatKhau,
                HieuLuc = p.HieuLuc,
                VaiTro = p.VaiTro
            }).Where(p => p.VaiTro == 0);
            return View(model);
        }

        [Route("EditUser")]
        [HttpGet]
        public IActionResult EditUser(string id)
        {
            var data = db.KhachHangs.SingleOrDefault(p => p.MaKh == id);

            if (data == null)
            {
                TempData["Message"] = $"Khong Thay Hang Co Ma {id} Can Tim";
                return Redirect("/404");
            }

            var result = new UserVM
            {
                MaKh = data.MaKh,
                MatKhau = data.MatKhau,
                HieuLuc = data.HieuLuc,
                VaiTro = data.VaiTro
            };
            return View(result);
        }


        [Route("EditUser")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditUser(UserVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = db.KhachHangs.SingleOrDefault(p => p.MaKh == model.MaKh);
            result.HieuLuc = model.HieuLuc;
            result.VaiTro = model.VaiTro;

            db.SaveChanges();

            return RedirectToAction("EditUser", new {id = model.MaKh});
        }
    }
}
