using System.ComponentModel.DataAnnotations;
using TodosMvc.Models.Enums;

namespace TodosMvc.Models.ViewModels
{
    public class TodoVM
    {
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }
        [Required]
        public TodoStatus Status { get; set; } = TodoStatus.Pending;
    }
}
