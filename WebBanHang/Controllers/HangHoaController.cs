using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using WebBanHang.Data;
using WebBanHang.ViewModels;

namespace WebBanHang.Controllers
{
    public class HangHoaController : Controller
    {
        private readonly Hshop2023Context db;
        private readonly ITransformer _mlModel;
        private readonly MLContext _mlContext;

        public HangHoaController(Hshop2023Context context, ITransformer mlModel)
        {
            db = context;
            _mlModel = mlModel;
            _mlContext = new MLContext();
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

        //public IActionResult Detail(int id)
        //{
        //    var data = db.HangHoas
        //        .Include(p => p.MaLoaiNavigation)
        //        .SingleOrDefault(p => p.MaHh == id);
        //    if (data == null)
        //    {
        //        TempData["Message"] = $"Khong Thay Hang Co Ma {id} Can Tim";
        //        return Redirect("/404");
        //    }
        //    var result = new ChiTietHangHoaVM
        //    {
        //        MaHang = data.MaHh,
        //        TenHang = data.TenHh,
        //        DonGiaHang = data.DonGia ?? 0,
        //        ChiTietHang = data.MoTa ?? "",
        //        HinhHang = data.Hinh ?? "",
        //        MoTaNganHang = data.MoTaDonVi ?? "",
        //        TenLoaiHang = data.MaLoaiNavigation.TenLoai,
        //        SoLuongTon = 10,
        //        DiemDanhGiaHang = 5,
        //    };
        //    return View(result);
        //}

        //public IActionResult Detail(int id)
        //{
        //    // Fetch product details
        //    var product = db.HangHoas
        //        .Include(p => p.MaLoaiNavigation)
        //        .SingleOrDefault(p => p.MaHh == id);

        //    if (product == null)
        //    {
        //        TempData["Message"] = $"Product with ID {id} not found";
        //        return Redirect("/404");
        //    }

        //    // Convert HangHoa to ChiTietHangHoaVM for main product display
        //    var result = new ChiTietHangHoaVM
        //    {
        //        MaHang = product.MaHh,
        //        TenHang = product.TenHh,
        //        DonGiaHang = product.DonGia ?? 0,
        //        ChiTietHang = product.MoTa ?? "",
        //        HinhHang = product.Hinh ?? "",
        //        MoTaNganHang = product.MoTaDonVi ?? "",
        //        TenLoaiHang = product.MaLoaiNavigation.TenLoai,
        //        SoLuongTon = 10,  // Sample data
        //        DiemDanhGiaHang = 5  // Sample data
        //    };

        //    // Prepare the sample data for the ML.NET model prediction
        //    var sampleData = new MyRecommendation.ModelInput
        //    {
        //        MaHH = (float)product.MaHh,   // Current product ID
        //        MaKH = "ALL_USERS",  // We can use a placeholder to indicate "all users"
        //    };

        //    // Use the ML.NET model to predict a related product
        //    var predictedResult = MyRecommendation.Predict(sampleData);  // This returns a single prediction, not a collection

        //    // Access the predicted product ID
        //    var predictedProductId = (int)predictedResult.MaHH;

        //    // Ensure the predicted product is not the same as the current product
        //    if (predictedProductId != product.MaHh)
        //    {
        //        // Fetch the recommended product from the database using the predicted product ID
        //        var recommendedProduct = db.HangHoas.SingleOrDefault(hh => hh.MaHh == predictedProductId);

        //        if (recommendedProduct != null)
        //        {
        //            ViewBag.RecommendedProducts = new List<HangHoa> { recommendedProduct };
        //        }
        //    }

        //    // If the prediction is invalid or no related products are found, fall back to fetching products from the same category
        //    if (ViewBag.RecommendedProducts == null || !ViewBag.RecommendedProducts.Any())
        //    {
        //        var fallbackProducts = db.HangHoas
        //            .Where(p => p.MaLoai == product.MaLoai && p.MaHh != id)  // Exclude the current product
        //            .Take(20)  // Show up to 4 fallback products
        //            .ToList();

        //        ViewBag.RecommendedProducts = fallbackProducts;
        //    }

        //    return View(result);  // Pass the main product details to the view
        //}


        //public IActionResult Detail(int id)
        //{
        //    // Fetch product details
        //    var product = db.HangHoas
        //        .Include(p => p.MaLoaiNavigation)
        //        .SingleOrDefault(p => p.MaHh == id);

        //    if (product == null)
        //    {
        //        TempData["Message"] = $"Product with ID {id} not found";
        //        return Redirect("/404");
        //    }

        //    var result = new ChiTietHangHoaVM
        //    {
        //        MaHang = product.MaHh,
        //        TenHang = product.TenHh,
        //        DonGiaHang = product.DonGia ?? 0,
        //        ChiTietHang = product.MoTa ?? "",
        //        HinhHang = product.Hinh ?? "",
        //        MoTaNganHang = product.MoTaDonVi ?? "",
        //        TenLoaiHang = product.MaLoaiNavigation.TenLoai,
        //        SoLuongTon = 10,  // Sample data
        //        DiemDanhGiaHang = 5  // Sample data
        //    };

        //    // Simulate getting the user's ID from session or authentication
        //    string currentUserId;

        //    if (User.Identity.IsAuthenticated) // Assuming you use authentication
        //    {
        //        currentUserId = User.Identity.Name; // Use the actual customer ID
        //    }
        //    else
        //    {
        //        // New customer (no history), use a generic user ID
        //        currentUserId = "NEW_USER";  // Generic placeholder for new customers
        //    }

        //    // Prepare the input for the ML.NET model
        //    var sampleData = new MyRecommendation.ModelInput
        //    {
        //        MaHH = (float)product.MaHh,   // The current product ID
        //        MaKH = currentUserId          // Use real user ID or generic "NEW_USER"
        //    };

        //    // Use the ML.NET model to predict related products
        //    var predictedResult = MyRecommendation.Predict(sampleData);

        //    // Access the predicted product ID
        //    var predictedProductId = (int)predictedResult.MaHH;

        //    // Check if the predicted product is not the same as the current product
        //    if (predictedProductId != product.MaHh)
        //    {
        //        // Fetch the recommended product from the database using the predicted product ID
        //        var recommendedProduct = db.HangHoas.SingleOrDefault(p => p.MaHh == predictedProductId);

        //        if (recommendedProduct != null)
        //        {
        //            ViewBag.RecommendedProducts = new List<HangHoa> { recommendedProduct };
        //        }
        //    }

        //    // If the prediction is invalid or the recommended product is the same, fall back to fetching products from the same category
        //    if (ViewBag.RecommendedProducts == null || !ViewBag.RecommendedProducts.Any())
        //    {
        //        var fallbackProducts = db.HangHoas
        //            .Where(p => p.MaLoai == product.MaLoai && p.MaHh != id)  // Exclude the current product
        //            .Take(4)  // Show up to 4 fallback products
        //            .ToList();

        //        ViewBag.RecommendedProducts = fallbackProducts;
        //    }

        //    return View(result);  // Pass the main product details and recommended products to the view
        //}
        public IActionResult Detail(int id)
        {
            // Fetch product details
            var product = db.HangHoas
                .Include(p => p.MaLoaiNavigation)
                .SingleOrDefault(p => p.MaHh == id);

            if (product == null)
            {
                TempData["Message"] = $"Product with ID {id} not found";
                return Redirect("/404");
            }

            // Convert HangHoa to ChiTietHangHoaVM for main product display
            var result = new ChiTietHangHoaVM
            {
                MaHang = product.MaHh,
                TenHang = product.TenHh,
                DonGiaHang = product.DonGia ?? 0,
                ChiTietHang = product.MoTa ?? "",
                HinhHang = product.Hinh ?? "",
                MoTaNganHang = product.MoTaDonVi ?? "",
                TenLoaiHang = product.MaLoaiNavigation.TenLoai,
                SoLuongTon = 10,  // Sample data
                DiemDanhGiaHang = 5  // Sample data
            };

            // Fetch the category ID of the product
            var currentCategoryId = product.MaLoai;

            // Define related categories mapping (example)
            var relatedCategories = new Dictionary<int, List<int>>
    {
        { 1, new List<int> { 2, 3 } },  // Phones are related to Laptops (2) and Cameras (3)
        { 4, new List<int> { 5, 6 } },  // Fragrances are related to Jewelry (5) and Shoes (6)
        // Add more mappings as needed
    };

            // Check if the current product category has related categories
            List<int> relatedCategoryIds;
            if (relatedCategories.TryGetValue(currentCategoryId, out relatedCategoryIds))
            {
                // Fetch random products from related categories (excluding the current product category)
                var randomRelatedProducts = db.HangHoas
                    .Where(p => relatedCategoryIds.Contains(p.MaLoai) && p.MaHh != id)
                    .OrderBy(r => Guid.NewGuid())  // Randomize the selection
                    .Take(4)  // Limit to 4 random products
                    .ToList();

                ViewBag.RecommendedProducts = randomRelatedProducts;
            }
            else
            {
                // Fallback: If no related categories, recommend random products from all categories
                var fallbackProducts = db.HangHoas
                    .Where(p => p.MaHh != id)
                    .OrderBy(r => Guid.NewGuid())  // Randomize fallback selection
                    .Take(20)  // Limit to 4 random products
                    .ToList();

                ViewBag.RecommendedProducts = fallbackProducts;
            }

            return View(result);
        }

    }
}
