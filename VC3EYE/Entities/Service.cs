using System;
using System.Collections.Generic;

namespace VC3EYE.Entities;

public partial class Service
{
    public int ServiceId { get; set; }

    public string ServiceName { get; set; } = null!;

    public string? Url { get; set; }

    public string Location { get; set; } = null!;

    public string? Ipaddress { get; set; }

    public string? GeneralInformation { get; set; }

    public bool IsImportant { get; set; }

    public bool IsActive { get; set; }

    public int? Port { get; set; }

    public string? RssfeedLink { get; set; }

    public TimeSpan CheckInterval { get; set; }

    public int? NumOfDownTimes { get; set; }

    public int? NumOfBackup { get; set; }

    public DateTime DateAdded { get; set; }

    public DateTime LastDateModified { get; set; }

    public DateTime LastTimeChecked { get; set; }

    public bool IsRunning { get; set; }

    public bool IsInternal { get; set; }

    public int UserId { get; set; }

    public string? NotificationMessage { get; set; }

    public bool IsDeleted { get; set; }

    public bool NotifyByMsteams { get; set; }

    public string? LookupTerm { get; set; }

    public bool IsIcmpRunning { get; set; }

    public int RunningDownCounter { get; set; }

    public virtual ICollection<ServiceDownHistory> ServiceDownHistories { get; } = new List<ServiceDownHistory>();

    public virtual User User { get; set; } = null!;
}
