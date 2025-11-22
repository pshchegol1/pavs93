namespace LoginDashboardApp.Models
{
    public class DashboardViewModel
    {
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public DateTime LoginTime { get; set; }
        public DashboardStats Stats { get; set; } = new DashboardStats();
        public List<string> RecentActivities { get; set; } = new List<string>();
    }

    public class DashboardStats
    {
        public decimal TotalRevenue { get; set; } = 45678.00m;
        public int TotalUsers { get; set; } = 1234;
        public int ActiveProjects { get; set; } = 12;
        public int PendingTasks { get; set; } = 5;
    }
}
