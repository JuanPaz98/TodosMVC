using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TodosMvc.Models.Enums;

namespace TodosMvc.Models;

public partial class Todo
{
    public int Todoid { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }
    [EnumDataType(typeof(TodoStatus))]
    public string Status { get; set; } = TodoStatus.Pending.ToString();

    public DateTime? Duedate { get; set; }

    public int Userid { get; set; }

    public DateTime Createdat { get; set; }

    public virtual User? User { get; set; }
}
