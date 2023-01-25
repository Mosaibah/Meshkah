using System;
using System.Collections.Generic;

namespace Meshkah.Models;

public partial class PointsTransaction
{
    public int Id { get; set; }

    public int? PointId { get; set; }

    public int? UserId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Point? Point { get; set; }

    public virtual User? User { get; set; }
}
