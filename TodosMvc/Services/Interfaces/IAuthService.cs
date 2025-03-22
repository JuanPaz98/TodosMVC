using TodosMvc.Models.ViewModels;

namespace TodosMvc.Services.Interfaces
{
    public interface IAuthService
    {
        Task<string> GenerateJwtToken(LoginVm model);
        Task LogoutAsync();
    }
}
