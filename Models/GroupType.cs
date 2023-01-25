using System;
using System.Collections.Generic;

namespace Meshkah.Models;

public partial class GroupType
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Group> Groups { get; } = new List<Group>();
}
