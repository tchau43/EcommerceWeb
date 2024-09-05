namespace WebBanHang.ViewModels
{
    public class CartItem
    {
        public int MaHang { get; set; }
        public string HinhHang { get; set;}
        public string TenHang { get; set;}
        public int SoLuongHang {  get; set;}
        public double DonGiaHang { get; set; }
        public double ThanhTien => DonGiaHang * SoLuongHang;
    }
}