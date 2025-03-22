using System.ComponentModel.DataAnnotations;

namespace TodosMvc.Models.ViewModels
{
    public class LoginVm
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
