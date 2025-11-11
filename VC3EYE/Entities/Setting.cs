using System;
using System.Collections.Generic;

namespace VC3EYE.Entities;

public partial class Setting
{
    public int SettingId { get; set; }

    public int? SmtpPort { get; set; }

    public string? SmtpUserName { get; set; }

    public string? SmtpPassword { get; set; }

    public string? MtClientId { get; set; }

    public string? MtSecretKey { get; set; }

    public DateTime DateAdded { get; set; }

    public virtual ICollection<User> Users { get; } = new List<User>();
}
