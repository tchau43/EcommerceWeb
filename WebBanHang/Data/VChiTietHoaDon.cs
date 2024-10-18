using System;
using System.Collections.Generic;

namespace WebBanHang.Data;

public partial class VChiTietHoaDon
{
    public int MaCt { get; set; }
    public int MaHd { get; set; }
    public int MaHh { get; set; }
    public double DonGia { get; set; }
    public int SoLuong { get; set; }
    public double GiamGia { get; set; }
    public string TenHh { get; set; } = null!;

    // New attributes to be added
    public int? MaKH { get; set; }  // Nullable, as not all records may have a customer ID
    public DateTime? NgayDat { get; set; }  // Nullable date for the transaction date
    public double? TongTien { get; set; }  // Nullable to represent total amount for the product
}

