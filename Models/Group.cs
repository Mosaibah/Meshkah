using System;
using System.Collections.Generic;

namespace Meshkah.Models;

public partial class Group
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public int? GroupTypeId { get; set; }

    public virtual GroupType? GroupType { get; set; }

    public virtual ICollection<UserGroupMapping> UserGroupMappings { get; } = new List<UserGroupMapping>();
}
