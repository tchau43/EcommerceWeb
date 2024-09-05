using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebBanHang.Data;
using WebBanHang.Helpers;
using WebBanHang.ViewModels;


namespace WebBanHang.Controllers
{
    public class KhachHangController : Controller
    {
        private readonly Hshop2023Context db;
        private readonly IMapper _mapper;

        public KhachHangController(Hshop2023Context context, IMapper mapper)
        {
            db = context;
            _mapper = mapper;
        }

        #region Register

        [HttpGet]
        public IActionResult DangKy()
        {
            return View();
        }


        //[HttpPost]
        ////[ValidateAntiForgeryToken]
        //public IActionResult DangKy(RegisterVM model, IFormFile Hinh)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            // Map data from RegisterVM to KhachHang entity
        //            var khachHang = _mapper.Map<KhachHang>(model);
        //            //khachHang.RandomKey = MyUtil.GenerateRandomKey();
        //            //khachHang.MatKhau = model.MatKhau.ToMd5Hash(khachHang.RandomKey);
        //            khachHang.HieuLuc = true;
        //            khachHang.VaiTro = 0;

        //            // Upload image if provided
        //            //if (Hinh != null)
        //            //{
        //            //    khachHang.Hinh = MyUtil.UploadHinh(Hinh, "KhachHang");
        //            //}
        //            // Add the entity to the database
        //            db.Add(khachHang);
        //            db.SaveChanges();
        //            return RedirectToAction("Index", "HangHoa");
        //        }
        //        catch (Exception ex)
        //        {
        //            // Handle exception if any
        //            // Log the exception or show an error message
        //        }
        //    }
        //    return View();
        //}

        [HttpPost]
        public IActionResult DangKy(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var khachHang = _mapper.Map<KhachHang>(model);
                    //khachHang.RandomKey = MyUtil.GenerateRandomKey();
                    //khachHang.MatKhau = model.MatKhau.ToMd5Hash(khachHang.RandomKey);
                    khachHang.HieuLuc = true;//sẽ xử lý khi dùng Mail để active
                    khachHang.VaiTro = 0;

                    //if (Hinh != null)
                    //{
                    //    khachHang.Hinh = MyUtil.UploadHinh(Hinh, "KhachHang");
                    //}

                    db.Add(khachHang);
                    db.SaveChanges();
                    return RedirectToAction("Index", "HangHoa");
                }
                catch (Exception ex)
                {
                    var mess = $"{ex.Message} shh";
                }
            }
            return View();
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,Name,Email,Password,Role")] User user)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(user);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(user);
        //}

        #endregion

        #region Login
        [HttpGet]
        public IActionResult Login(string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(ClientLoginVM model, string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            if (ModelState.IsValid)
            {
                var khachHang = db.KhachHangs.SingleOrDefault(kh => kh.MaKh == model.UserName);
                if (khachHang == null)
                {
                    ModelState.AddModelError("Error", "Sai Ten Dang Nhap");
                }
                else if (!khachHang.HieuLuc)
                {
                    ModelState.AddModelError("Error", "Tai Khoan Bi Khoa. Vui Long Lien He Admin");
                }
                //else if(khachHang.MatKhau != model.Password.ToMd5Hash(khachHang.RandomKey))
                else if (khachHang.MatKhau != model.Password)

                {
                    ModelState.AddModelError("Error", "Sai Mat Khau, Vui Long Thu Lai");
                }
                else
                {
                    var claims = new List<Claim>() {
                        new Claim(ClaimTypes.Name, khachHang.HoTen), // Make sure 'HoTen' contains the admin's name.
                        new Claim(ClaimTypes.Email, khachHang.Email),
                        new Claim(ClaimTypes.Email, khachHang.Email),
                        new Claim(MySetting.CLAIM_CUSTOMER_ID, khachHang.MaKh),
                        new Claim(ClaimTypes.Role, khachHang.VaiTro == 1 ? "Admin" : "User"),
                    };
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsPricipal = new ClaimsPrincipal(claimsIdentity);
                    await HttpContext.SignInAsync(claimsPricipal);

                    if (khachHang.VaiTro == 1)
                    {
                        return RedirectToAction("Index", "Home", new { area = "Admin" });
                    }
                    else
                    {
                        if (Url.IsLocalUrl(ReturnUrl))
                        {
                            return Redirect(ReturnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Profile", "KhachHang");
                        }
                    }
                }
            }
            return View();
        }
        #endregion

        #region User Profile

        [Authorize]
        public IActionResult Profile()
        {
            return View();
        }

        #endregion

        #region Logout

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }
        #endregion

    }
}
