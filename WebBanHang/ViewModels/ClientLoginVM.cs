using System.ComponentModel.DataAnnotations;

namespace WebBanHang.ViewModels
{
    public class ClientLoginVM
    {
        [Display(Name = "Ten Dang Nhap")]
        [Required(ErrorMessage = "Chua Dien Ten Dang Nhap")]
        [MaxLength(20, ErrorMessage = "Qua Do Dai Cho Phep: 20")]
        public string UserName { get; set; }

        [Display(Name = "Mat Khau")]
        [Required(ErrorMessage = "Chua Dien Mat Khau")]
        [MaxLength(50, ErrorMessage = "Qua Do Dai Cho Phep: 50")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
