using System;
using System.Collections.Generic;

namespace VC3EYE.Entities;

public partial class ErrorLog
{
    public int ErrorLogId { get; set; }

    public string ErrorName { get; set; } = null!;

    public string ErrorDescription { get; set; } = null!;

    public DateTime DateAdded { get; set; }

    public bool IsArchived { get; set; }
}
