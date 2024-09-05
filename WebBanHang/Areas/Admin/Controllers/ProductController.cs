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
    [Route("Product")]
    public class ProductController : Controller
    {
        private readonly Hshop2023Context db;
        private readonly ILogger<ProductController> _logger;
        private readonly IWebHostEnvironment environment;

        public ProductController(Hshop2023Context context, ILogger<ProductController> logger, IWebHostEnvironment environment)
        {
            db = context;
            _logger = logger;
            this.environment = environment;
        }

        [Route("Edit")]
        public IActionResult Edit(int id)
        {
            var data = db.HangHoas
                .Include(p => p.MaLoaiNavigation)
                .SingleOrDefault(p => p.MaHh == id);
            if (data == null)
            {
                TempData["Message"] = $"Khong Thay Hang Co Ma {id} Can Tim";
                return Redirect("/404");
            }
            var result = new EditProductVM
            {
                MaHang = data.MaHh,
                TenHang = data.TenHh,
                DonGiaHang = data.DonGia ?? 0,
                ChiTietHang = data.MoTa ?? "",
                //HinhHang = data.Hinh ?? "",
                MoTaNganHang = data.MoTaDonVi ?? "",
                TenLoaiHang = data.MaLoaiNavigation.MaLoai,
            };
            ViewBag.MaLoai = new SelectList(db.Loais, "MaLoai", "TenLoai");
            ViewData["HinhHang"] = data.Hinh;

            return View(result);
        }


        [Route("Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(EditProductVM viewModel, IFormFile Hinh)
        {
            if (!ModelState.IsValid)
            {
                // Logging the error
                _logger.LogError("Model state is invalid for editing product with ID: {ID}", viewModel.MaHang);
                return View(viewModel);
            }
            try
            {
                var product = db.HangHoas.Include(p => p.MaNccNavigation).SingleOrDefault(p => p.MaHh == viewModel.MaHang);

                product.TenHh = viewModel.TenHang;
                product.DonGia = viewModel.DonGiaHang;
                product.MoTa = viewModel.ChiTietHang;
                product.MoTaDonVi = viewModel.MoTaNganHang;
                product.MaLoai = viewModel.TenLoaiHang;

                product.Hinh = MyUtil.UploadHinh(Hinh, "HangHoa");

                db.SaveChanges();
                TempData["Success"] = "Product updated successfully.";
                return RedirectToAction("Edit", new { id = viewModel.MaHang });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product with ID: {ID}", viewModel.MaHang);
                TempData["Error"] = "An error occurred while updating the product.";
                return View(viewModel);
            }
        }



        [Route("Create")]
        public IActionResult Create()
        {
            ViewBag.MaLoai = new SelectList(db.Loais, "MaLoai", "TenLoai");
            return View();
        }

        [Route("Create")]
        [HttpPost]
        public IActionResult Create(CreateProductVM model, IFormFile Hinh)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            HangHoa product = new HangHoa()
            {
                TenHh = model.TenHang,
                DonGia = model.DonGiaHang,
                MoTaDonVi = model.MoTaNganHang,
                MoTa = model.ChiTietHang,
                MaLoai = model.TenLoaiHang,
                MaNcc = "AP",
            };

            if (Hinh != null)
            {
                product.Hinh = MyUtil.UploadHinh(Hinh, "HangHoa");
            }

            db.HangHoas.Add(product);
            db.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        [Route("Delete")]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            using var transaction = db.Database.BeginTransaction();
            try
            {
                // Find the product by ID
                var product = db.HangHoas.Include(p => p.MaLoaiNavigation).SingleOrDefault(p => p.MaHh == id);

                // Check if the product exists
                if (product == null)
                {
                    TempData["Error"] = $"Product with ID {id} not found.";
                    return RedirectToAction("Index", "Home");
                }

                var relatedOrderDetails = db.ChiTietHds.Where(od => od.MaHhNavigation.MaHh == id).ToList();
                db.ChiTietHds.RemoveRange(relatedOrderDetails);

                // Remove the product from the database
                db.HangHoas.Remove(product);

                // Save the changes to the database
                db.SaveChanges();
                transaction.Commit();

                // Set success message and redirect to the desired action
                TempData["Success"] = "Product deleted successfully.";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                // Log the error and set an error message
                _logger.LogError(ex, "Error deleting product with ID: {ID}", id);
                TempData["Error"] = "An error occurred while deleting the product.";
                return RedirectToAction("Index", "Home");
            }
        }

    }
}
