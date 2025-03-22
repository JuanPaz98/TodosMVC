using System.ComponentModel.DataAnnotations;

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
        public string Status {  get; set; }
    }
}
