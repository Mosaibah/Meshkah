using System;
using System.Collections.Generic;

namespace Meshkah.Models;

public partial class PonitType
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Point> Points { get; } = new List<Point>();
}
