namespace LoginDashboardApp.Models
{
    public class ErrorLog
    {
        public int ErrorLogId { get; set; }

        public string ErrorName { get; set; } = null!;

        public string ErrorDescription { get; set; } = null!;

        public DateTime DateAdded { get; set; }

        public bool IsArchived { get; set; }
    }
}
