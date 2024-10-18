using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebBanHang.Migrations
{
    /// <inheritdoc />
    public partial class AddNewFieldsToVChiTietHoaDon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the existing view if it exists
            migrationBuilder.Sql(@"DROP VIEW IF EXISTS [dbo].[vChiTietHoaDon];");

            // Recreate the view with the new fields
            migrationBuilder.Sql(@"
                CREATE VIEW [dbo].[vChiTietHoaDon]
                AS 
                SELECT cthd.*, hh.TenHH, hd.MaKH, hd.NgayDat, (cthd.SoLuong * cthd.DonGia) AS TongTien
                FROM ChiTietHD cthd
                JOIN HangHoa hh ON hh.MaHH = cthd.MaHH
                JOIN HoaDon hd ON hd.MaHD = cthd.MaHD;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Rollback to the previous state of the view
            migrationBuilder.Sql(@"DROP VIEW IF EXISTS [dbo].[vChiTietHoaDon];");

            // You can recreate the original view here, or leave it dropped
            migrationBuilder.Sql(@"
                CREATE VIEW [dbo].[vChiTietHoaDon]
                AS 
                SELECT cthd.*, hh.TenHH
                FROM ChiTietHD cthd
                JOIN HangHoa hh ON hh.MaHH = cthd.MaHH;
            ");
        }

    }
}
