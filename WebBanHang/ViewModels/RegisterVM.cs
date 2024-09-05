using System.ComponentModel.DataAnnotations;

namespace WebBanHang.ViewModels
{
    public class RegisterVM
    {
        [Display(Name = "Ten Dang Nhap")]
        [Required(ErrorMessage = "Vui long nhap")]
        [MaxLength(20, ErrorMessage = "Toi da 20 ki tu")]
        public string MaKh { get; set; }

        [Required(ErrorMessage = "Vui long nhap")]
        [Display(Name = "Mat Khau")]
        [DataType(DataType.Password)]
        public string MatKhau { get; set; }

        [Required(ErrorMessage = "Vui long nhap")]
        [MaxLength(50, ErrorMessage = "Toi da 50 ki tu")]
        [Display(Name = "Ho Ten")]
        public string HoTen { get; set; }

        public bool GioiTinh { get; set; } = true;

        [Display(Name = "Ngay Sinh")]
        [DataType(DataType.Date)]
        public DateTime? NgaySinh { get; set; }

        [Required(ErrorMessage = "Vui long nhap")]
        [MaxLength(60, ErrorMessage = "Toi da 60 ki tu")]
        [Display(Name = "Dia Chi")]
        public string DiaChi { get; set; }

        [Required(ErrorMessage = "Vui long nhap")]
        [MaxLength(10, ErrorMessage = "Toi da 10 ki tu")]
        [RegularExpression(@"0[37985]\d{8}", ErrorMessage = "Chua dung dinh dang")]
        [Display(Name = "Dien Thoai")]
        public string DienThoai { get; set; }

        //[Required(ErrorMessage = "Vui long nhap")]
        [EmailAddress(ErrorMessage = "Chua dung dinh dang")]
        [Display(Name = "Email")]
        public string Email { get; set; } = null!;

        public string? Hinh { get; set; }
    }
}
