namespace WebBanHang.Areas.Admin.ViewModels
{
    public class RevenueVM
    {
        public double TotalRevenue { get; set; }
        public double SelectedPeriodRevenue { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        // New properties for graph data
        public List<string> Labels { get; set; } // For date labels (X-axis)
        public List<double> RevenueData { get; set; } // For corresponding revenue (Y-axis)
    }

}
