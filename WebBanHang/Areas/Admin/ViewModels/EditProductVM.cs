using System.ComponentModel.DataAnnotations;

namespace WebBanHang.Areas.Admin.ViewModels
{
    public class EditProductVM
    {
        public int MaHang { get; set; }

        [Required(ErrorMessage = "Vui long nhap")]
        public string TenHang { get; set; }

        public IFormFile? HinhHang { get; set; }

        [Required(ErrorMessage = "Vui long nhap")]
        public double DonGiaHang { get; set; }

        [Required(ErrorMessage = "Vui long nhap")]
        public string MoTaNganHang { get; set; }

        [Required(ErrorMessage = "Vui long nhap")]
        public int TenLoaiHang { get; set; }

        [Required(ErrorMessage = "Vui long nhap")]
        public string ChiTietHang { get; set; }
    }

}