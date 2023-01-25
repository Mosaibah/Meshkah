using System;
using System.Collections.Generic;

namespace Meshkah.Models;

public partial class MoneyMovement
{
    public int Id { get; set; }

    public int? PointId { get; set; }

    public int? MoneyTransactionId { get; set; }

    public int UserId { get; set; }

    public decimal Amount { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual MoneyTransaction? MoneyTransaction { get; set; }

    public virtual Point? Point { get; set; }

    public virtual User User { get; set; } = null!;
}
