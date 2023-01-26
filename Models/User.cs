using System;
using System.Collections.Generic;

namespace Meshkah.Models;

public partial class User
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<MoneyMovement> MoneyMovements { get; } = new List<MoneyMovement>();

    public virtual ICollection<MoneyTransaction> MoneyTransactionFromUsers { get; } = new List<MoneyTransaction>();

    public virtual ICollection<MoneyTransaction> MoneyTransactionToUsers { get; } = new List<MoneyTransaction>();

    public virtual ICollection<PointsTransaction> PointsTransactions { get; } = new List<PointsTransaction>();

    public virtual ICollection<UserGroupMapping> UserGroupMappings { get; } = new List<UserGroupMapping>();

    public virtual ICollection<UserRoleMapping> UserRoleMappings { get; } = new List<UserRoleMapping>();
}
