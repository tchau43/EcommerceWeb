using System.ComponentModel.DataAnnotations;

namespace WebBanHang.ViewModels
{
    public class HangHoaVM
    {
        public int MaHang {  get; set; }
        public string TenHang { get; set;}
        public string HinhHang { get; set;}
        public double DonGiaHang { get; set;}
        public string MoTaNganHang { get;set; }
        public string TenLoaiHang { get; set; }
    }
    public class ChiTietHangHoaVM
    {
        public int MaHang { get; set; }
        public string TenHang { get; set; }
        public string HinhHang { get; set; }
        public double DonGiaHang { get; set; }
        public string MoTaNganHang { get; set; }
        public string TenLoaiHang { get; set; }
        public string ChiTietHang { get; set; }
        public int DiemDanhGiaHang { get; set; }
        public int SoLuongTon { get; set; }
    }
    //public class ChiTietHangHoaVM
    //{
    //    [Required]
    //    public int MaHang { get; set; }

    //    [Required]
    //    [StringLength(100, MinimumLength = 3)]
    //    public string TenHang { get; set; }

    //    [Range(1, 100000)]
    //    public decimal DonGiaHang { get; set; }

    //    public string ChiTietHang { get; set; }
    //    public string HinhHang { get; set; }
    //    public string MoTaNganHang { get; set; }

    //    // Other properties
    //}

}
