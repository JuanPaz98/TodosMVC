using System;
using System.Collections.Generic;

namespace TodosMvc.Models;

public partial class UsersRole
{
    public int Userid { get; set; }

    public int Roleid { get; set; }

    public virtual Role Role { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
