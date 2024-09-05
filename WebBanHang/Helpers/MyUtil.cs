using Microsoft.EntityFrameworkCore;
using System.Text;

namespace WebBanHang.Helpers
{
    public class MyUtil
    {
        public static string UploadHinh(IFormFile Hinh, String folder)
        {
            try
            {
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Hinh", folder, Hinh.FileName);
                // Kiểm tra xem file đã tồn tại chưa, nếu có thì xóa đi
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }

                using (var myFile = new FileStream(fullPath, FileMode.CreateNew))
                {
                    Hinh.CopyTo(myFile);
                }
                return Hinh.FileName;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }

        }
        public static string GenerateRandomKey(int length = 5)
        {
            var pattern = @"qwertyuiopasdfghjklzxcvbbnmQWERTYUIOPASDFGHJKLZXCVBBNM!@#$%^&*";
            var rd = new Random();
            var sb = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                sb.Append(pattern[rd.Next(0, pattern.Length)]);
            }
            return sb.ToString();
        }
        //public decimal CalculateTotalRevenue(DateTime startDate, DateTime endDate)
        //{
        //    return _context.HoaDons
        //        .Where(hd => hd.NgayGiao >= startDate && hd.NgayGiao <= endDate && hd.MaTrangThai == 2)  // Assuming '2' signifies a completed transaction
        //        .SelectMany(hd => hd.ChiTietHds)
        //        .Sum(cthd => cthd.Quantity * cthd.UnitPrice);  // Adjust this calculation as per your model's properties
        //}
    }
}
