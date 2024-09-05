namespace WebBanHang.Areas.Admin.ViewModels
{
    public class RevenueVM
    {
        public double TotalRevenue { get; set; }
        public double SelectedPeriodRevenue { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

    }
}
