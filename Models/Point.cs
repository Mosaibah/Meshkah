using System;
using System.Collections.Generic;

namespace Meshkah.Models;

public partial class Point
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public decimal? Amount { get; set; }

    public int? TypeId { get; set; }

    public virtual ICollection<MoneyMovement> MoneyMovements { get; } = new List<MoneyMovement>();

    public virtual ICollection<PointsTransaction> PointsTransactions { get; } = new List<PointsTransaction>();

    public virtual PonitType? Type { get; set; }
}
