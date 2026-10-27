using System.Threading.Tasks;
using Champions.Data.DTOs;

namespace Champions.Services
{
    /// <summary>Wraps GDK IAuthenticate; exposes Champions-specific auth operations.</summary>
    public interface IAuthService
    {
        bool IsLoggedIn { get; }
        string AccessToken { get; }

        /// <summary>POST /login</summary>
        Task<AuthResponse> LoginAsync(LoginRequest request);

        /// <summary>POST /signup</summary>
        Task<AuthResponse> RegisterAsync(RegisterRequest request);

        /// <summary>POST /forgot-password</summary>
        Task ForgotPasswordAsync(ForgotPasswordRequest request);

        /// <summary>POST /logout</summary>
        Task LogoutAsync();

        /// <summary>Refresh token silently.</summary>
        Task<bool> TryRefreshAsync();
    }
}
