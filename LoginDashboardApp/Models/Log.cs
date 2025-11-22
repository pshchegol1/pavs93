namespace LoginDashboardApp.Models
{
    public class Log
    {
        public int LogId { get; set; }

        public string LogName { get; set; } = null!;

        public string LogDescription { get; set; } = null!;

        public DateTime DateAdded { get; set; }

        public bool IsAudit { get; set; }

        public int UserId { get; set; }

        public virtual User User { get; set; } = null!;
    }
}
