namespace WebBanHang.ViewModels
{
    public class HoaDonVM
    {
        public int MaHd { get; set; }
        public string MaKh { get; set; }
        public DateTime NgayDat { get; set; }
        public DateTime? NgayCan { get; set; }
        public DateTime? NgayGiao { get; set; }
        public string HoTen { get; set; }
        public string DiaChi { get; set; }
        public string DienThoai { get; set; }
        public string CachThanhToan { get; set; }
        public string CachVanChuyen { get; set; }
        public double PhiVanChuyen { get; set; }
        public int MaTrangThai { get; set; }
        public string MaNv { get; set; }
        public string GhiChu { get; set; }

        // Additional ViewModel properties for revenue reporting
        public decimal TotalAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal ShippingAmount { get; set; }
        public decimal FinalAmount { get; set; }
        // You might want to include details from ChiTietHd if needed
        public List<ChiTietHdVM> ChiTietHds { get; set; }
        // Other relevant fields for reporting purposes
    }

    public class ChiTietHdVM
    {
        // Assuming you have properties like Quantity, UnitPrice in ChiTietHd
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SubTotal => Quantity * UnitPrice;
    }
}
