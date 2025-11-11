using System;
using System.Collections.Generic;

namespace VC3EYE.Entities;

public partial class Role
{
    public int RoleId { get; set; }

    public string Description { get; set; } = null!;

    public virtual ICollection<User> Users { get; } = new List<User>();
}
