using System;
using System.Collections.Generic;

namespace E_Library.Models;

public partial class Permission
{
    public int PermissionId { get; set; }

    public string? Link { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
