using System;
using System.Collections.Generic;

namespace Meshkah.Models;

public partial class MoneyTransaction
{
    public int Id { get; set; }

    public int FromUserId { get; set; }

    public int ToUserId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public decimal? Amount { get; set; }

    public virtual User FromUser { get; set; } = null!;

    public virtual ICollection<MoneyMovement> MoneyMovements { get; } = new List<MoneyMovement>();

    public virtual User ToUser { get; set; } = null!;
}
