using System;
using System.Collections.Generic;

namespace Meshkah.Models;

public partial class UserGroupMapping
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int? GroupId { get; set; }

    public virtual Group? Group { get; set; }

    public virtual User? User { get; set; }
}
