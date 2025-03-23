using TodosMvc.Models.ViewModels;

namespace TodosMvc.Services.Interfaces
{
    public interface IAuthService
    {
        public string GenerateJwtToken(LoginVm model);
        public void StoreJwtInCookie(string token);
        public void LogoutAsync();
    }
}
