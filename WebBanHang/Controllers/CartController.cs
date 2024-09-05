using Microsoft.AspNetCore.Mvc;
using WebBanHang.Data;
using WebBanHang.ViewModels;
using WebBanHang.Helpers;
using Microsoft.AspNetCore.Authorization;
using Stripe;
using Stripe.Checkout;

namespace WebBanHang.Controllers
{
    public class CartController : Controller
    {
        private readonly Hshop2023Context db;

        public CartController(Hshop2023Context context)
        {
            db = context;
        }

        public List<CartItem> Cart => HttpContext.Session.Get<List<CartItem>>(MySetting.CART_KEY) ?? new List<CartItem>();

        public IActionResult Index()
        {
            return View(Cart);
        }

        #region Add To Cart
        public IActionResult AddToCart(int id, int quantity = 1)
        {
            var gioHang = Cart;
            var item = gioHang.SingleOrDefault(p => p.MaHang == id);
            if (item == null)
            {
                var hangHoa = db.HangHoas.SingleOrDefault(p => p.MaHh == id);
                if (hangHoa == null)
                {
                    TempData["Message"] = $"Khong Tim Thay Hang Co Ma {id}";
                    return Redirect("/404");
                }
                item = new CartItem
                {
                    MaHang = hangHoa.MaHh,
                    TenHang = hangHoa.TenHh,
                    DonGiaHang = hangHoa.DonGia ?? 0,
                    HinhHang = hangHoa.Hinh ?? "",
                    SoLuongHang = quantity
                };
                gioHang.Add(item);
            }
            else
            {
                item.SoLuongHang += quantity;
            }
            HttpContext.Session.Set(MySetting.CART_KEY, gioHang);
            return RedirectToAction("Index");
        }

        public IActionResult RemoveCart(int id)
        {
            var gioHang = Cart;
            var item = gioHang.SingleOrDefault(p => p.MaHang == id);
            if (item != null)
            {
                gioHang.Remove(item);
                HttpContext.Session.Set(MySetting.CART_KEY, gioHang);
            }
            return RedirectToAction("Index");
        }

        #endregion

        #region Checkout

        [Authorize]
        [HttpGet]
        public IActionResult Checkout()
        {
            var gioHang = Cart;
            if (Cart.Count == 0)
            {
                return Redirect("/");
            }
            return View(Cart);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Checkout(CheckoutVM model)
        {
            if (ModelState.IsValid)
            {
                var customerId = HttpContext.User.Claims.SingleOrDefault(p => p.Type == MySetting.CLAIM_CUSTOMER_ID).Value;
                var khachHang = new KhachHang();

                if (model.GiongKhachHang)
                {
                    khachHang = db.KhachHangs.SingleOrDefault(p => p.MaKh == customerId);
                }
                var hoaDon = new HoaDon
                {
                    MaKh = customerId,
                    HoTen = model.FullName ?? khachHang.HoTen,
                    DiaChi = model.Address ?? khachHang.DiaChi,
                    DienThoai = model.PhoneNumber ?? khachHang.DienThoai,
                    NgayDat = DateTime.Now,
                    CachThanhToan = "COD",
                    CachVanChuyen = "GHTK",
                    MaTrangThai = 0,
                    GhiChu = model.Note,
                };

                db.Database.BeginTransaction();
                try
                {
                    db.Database.CommitTransaction();
                    db.Add(hoaDon);
                    db.SaveChanges();

                    var cthds = new List<ChiTietHd>();
                    foreach (var item in Cart)
                    {
                        cthds.Add(new ChiTietHd
                        {
                            MaHd = hoaDon.MaHd,
                            SoLuong = item.SoLuongHang,
                            DonGia = item.DonGiaHang,
                            MaHh = item.MaHang,
                            GiamGia = 0,
                        });
                    }
                    db.AddRange(cthds);
                    db.SaveChanges();

                    HttpContext.Session.Set<List<CartItem>>(MySetting.CART_KEY, new List<CartItem>());

                    return View("Success");
                }
                catch
                {
                    db.Database.RollbackTransaction();
                }
            }
            return View(Cart);
        }

        #endregion

        #region Stripe Payment

        public IActionResult OrderConfirmation(string session_id)
        {
            var service = new SessionService();
            Session session = service.Get(session_id);

            if (session.PaymentStatus == "paid")
            {
                // Lấy thông tin từ TempData
                var customerId = HttpContext.User.Claims.SingleOrDefault(p => p.Type == MySetting.CLAIM_CUSTOMER_ID)?.Value;
                var khachHang = db.KhachHangs.SingleOrDefault(p => p.MaKh == customerId) ?? new KhachHang();

                // Check if TempData contains the keys
                var fullName = TempData.ContainsKey("FullName") ? TempData["FullName"]?.ToString() : khachHang.HoTen;
                var address = TempData.ContainsKey("Address") ? TempData["Address"]?.ToString() : khachHang.DiaChi;
                var phoneNumber = TempData.ContainsKey("PhoneNumber") ? TempData["PhoneNumber"]?.ToString() : khachHang.DienThoai;
                var note = TempData.ContainsKey("Note") ? TempData["Note"]?.ToString() : string.Empty;

                var hoaDon = new HoaDon
                {
                    MaKh = customerId,
                    HoTen = fullName,
                    DiaChi = address,
                    DienThoai = phoneNumber,
                    NgayDat = DateTime.Now,
                    CachThanhToan = "VISA",
                    CachVanChuyen = "GHTK",
                    MaTrangThai = 1, // Đặt trạng thái đã thanh toán
                    GhiChu = note,
                };

                db.HoaDons.Add(hoaDon);
                db.SaveChanges();

                var cthds = Cart.Select(item => new ChiTietHd
                {
                    MaHd = hoaDon.MaHd,
                    MaHh = item.MaHang,
                    SoLuong = item.SoLuongHang,
                    DonGia = item.DonGiaHang,
                    GiamGia = 0,
                }).ToList();

                db.ChiTietHds.AddRange(cthds);
                db.SaveChanges();

                return View("Success");
            }
            return View("Failure");
        }

        [Authorize]
        [HttpPost]
        public IActionResult CheckOutStripe(CheckoutVM model)
        {
            if (ModelState.IsValid)
            {
                var customerId = HttpContext.User.Claims.SingleOrDefault(p => p.Type == MySetting.CLAIM_CUSTOMER_ID)?.Value;
                var khachHang = db.KhachHangs.SingleOrDefault(p => p.MaKh == customerId) ?? new KhachHang();

                // Check if the user selected "same as customer"
                if (model.GiongKhachHang)
                {
                    model.FullName = khachHang.HoTen;
                    model.Address = khachHang.DiaChi;
                    model.PhoneNumber = khachHang.DienThoai;
                }

                // Lưu thông tin hóa đơn tạm thời trong TempData
                TempData["FullName"] = model.FullName ?? khachHang.HoTen;
                TempData["Address"] = model.Address ?? khachHang.DiaChi;
                TempData["PhoneNumber"] = model.PhoneNumber ?? khachHang.DienThoai;
                TempData["Note"] = model.Note;

                var domain = "https://localhost:7270/";

                var options = new SessionCreateOptions
                {
                    SuccessUrl = domain + "Cart/OrderConfirmation?session_id={CHECKOUT_SESSION_ID}",
                    CancelUrl = domain + "Cart/Checkout",
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment"
                };

                foreach (var item in Cart)
                {
                    var sessionListItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.DonGiaHang * item.SoLuongHang * 100),
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.TenHang.ToString(),
                            }
                        },
                        Quantity = item.SoLuongHang
                    };
                    options.LineItems.Add(sessionListItem);
                }

                var service = new SessionService();
                Session session = service.Create(options);

                TempData["Session"] = session.Id;

                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
            }
            return View(Cart);
        }


        //public IActionResult OrderConfirmation(string session_id)
        //{
        //    var service = new SessionService();
        //    Session session = service.Get(session_id);

        //    if (session.PaymentStatus == "paid")
        //    {
        //        // Lấy thông tin từ TempData
        //        var customerId = HttpContext.User.Claims.SingleOrDefault(p => p.Type == MySetting.CLAIM_CUSTOMER_ID).Value;
        //        var khachHang = db.KhachHangs.SingleOrDefault(p => p.MaKh == customerId) ?? new KhachHang();

        //        var hoaDon = new HoaDon
        //        {
        //            MaKh = customerId,
        //            HoTen = TempData["FullName"].ToString(),
        //            DiaChi = TempData["Address"].ToString(),
        //            DienThoai = TempData["PhoneNumber"].ToString(),
        //            NgayDat = DateTime.Now,
        //            CachThanhToan = "VISA",
        //            CachVanChuyen = "GHTK",
        //            MaTrangThai = 1, // Đặt trạng thái đã thanh toán
        //            GhiChu = TempData["Note"].ToString(),
        //        };

        //        db.HoaDons.Add(hoaDon);
        //        db.SaveChanges();

        //        var cthds = Cart.Select(item => new ChiTietHd
        //        {
        //            MaHd = hoaDon.MaHd,
        //            MaHh = item.MaHang,
        //            SoLuong = item.SoLuongHang,
        //            DonGia = item.DonGiaHang,
        //            GiamGia = 0,
        //        }).ToList();

        //        db.ChiTietHds.AddRange(cthds);
        //        db.SaveChanges();

        //        return View("Success");
        //    }
        //    return View("Failure");
        //}

        //[Authorize]
        //[HttpPost]
        //public IActionResult CheckOutStripe(CheckoutVM model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var customerId = HttpContext.User.Claims.SingleOrDefault(p => p.Type == MySetting.CLAIM_CUSTOMER_ID).Value;
        //        var khachHang = db.KhachHangs.SingleOrDefault(p => p.MaKh == customerId) ?? new KhachHang();

        //        // Lưu thông tin hóa đơn tạm thời trong TempData
        //        TempData["FullName"] = model.FullName ?? khachHang.HoTen;
        //        TempData["Address"] = model.Address ?? khachHang.DiaChi;
        //        TempData["PhoneNumber"] = model.PhoneNumber ?? khachHang.DienThoai;
        //        TempData["Note"] = model.Note;

        //        var domain = "https://localhost:7270/";

        //        var options = new SessionCreateOptions
        //        {
        //            SuccessUrl = domain + "Cart/OrderConfirmation?session_id={CHECKOUT_SESSION_ID}",
        //            CancelUrl = domain + "Cart/Checkout",
        //            LineItems = new List<SessionLineItemOptions>(),
        //            Mode = "payment"
        //        };

        //        foreach (var item in Cart)
        //        {
        //            var sessionListItem = new SessionLineItemOptions
        //            {
        //                PriceData = new SessionLineItemPriceDataOptions
        //                {
        //                    UnitAmount = (long)(item.DonGiaHang * item.SoLuongHang * 100),
        //                    Currency = "usd",
        //                    ProductData = new SessionLineItemPriceDataProductDataOptions
        //                    {
        //                        Name = item.TenHang.ToString(),
        //                    }
        //                },
        //                Quantity = item.SoLuongHang
        //            };
        //            options.LineItems.Add(sessionListItem);
        //        }

        //        var service = new SessionService();
        //        Session session = service.Create(options);

        //        TempData["Session"] = session.Id;

        //        Response.Headers.Add("Location", session.Url);
        //        return new StatusCodeResult(303);
        //    }
        //    return View(Cart);
        //}


        #endregion



    }

}
