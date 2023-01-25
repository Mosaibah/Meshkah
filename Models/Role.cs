using System;
using System.Collections.Generic;

namespace Meshkah.Models;

public partial class Role
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<UserRoleMapping> UserRoleMappings { get; } = new List<UserRoleMapping>();
}
