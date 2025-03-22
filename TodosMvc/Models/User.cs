using System;
using System.Collections.Generic;

namespace TodosMvc.Models;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Passwordhash { get; set; } = null!;

    public DateTime Createdat { get; set; }

    public virtual ICollection<Todo> Todos { get; set; } = new List<Todo>();
}
