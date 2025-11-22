namespace LoginDashboardApp.Models
{
    public class ServiceDownHistory
    {
        public int ServiceDownHistorryId { get; set; }

        public int ServiceId { get; set; }

        public TimeSpan? ResponseTime { get; set; }

        public string? Htmlresponse { get; set; }

        public bool IsPortFailed { get; set; }

        public bool IsRssfeedFailed { get; set; }

        public bool IsPingFailed { get; set; }

        public DateTime DateAdded { get; set; }

        public DateTime? BackUpDateTime { get; set; }

        public virtual Service Service { get; set; } = null!;
    }
}
