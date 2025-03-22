using System;
using System.Collections.Generic;

namespace TodosMvc.Models;

public partial class Todo
{
    public int Todoid { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? Status { get; set; }

    public DateTime? Duedate { get; set; }

    public int Userid { get; set; }

    public DateTime Createdat { get; set; }

    public virtual User User { get; set; } = null!;
}
