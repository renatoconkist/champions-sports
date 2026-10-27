using System.Threading.Tasks;
using Champions.Data.DTOs;

namespace Champions.Services.Mock
{
    /// <summary>
    /// Fake auth service — returns hardcoded data so the UI can be exercised
    /// without a backend. Only compiled when CHAMPIONS_MOCK_SERVICES is defined.
    /// </summary>
#if CHAMPIONS_MOCK_SERVICES
    public class MockAuthService : IAuthService, IMockService
    {
        private const int FakeDelayMs = 600;

        public bool   IsLoggedIn   { get; private set; }
        public string AccessToken  { get; private set; }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            await Task.Delay(FakeDelayMs);
            IsLoggedIn  = true;
            AccessToken = "mock_access_abc123";
            return new AuthResponse
            {
                access_token  = AccessToken,
                refresh_token = "mock_refresh_abc123",
                token_type    = "Bearer",
                expires_in    = 3600
            };
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            await Task.Delay(FakeDelayMs);
            IsLoggedIn  = true;
            AccessToken = "mock_access_new123";
            return new AuthResponse
            {
                access_token  = AccessToken,
                refresh_token = "mock_refresh_new123",
                token_type    = "Bearer",
                expires_in    = 3600
            };
        }

        public async Task ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            await Task.Delay(FakeDelayMs);
            // No-op mock — success by default.
        }

        public async Task LogoutAsync()
        {
            await Task.Delay(200);
            IsLoggedIn  = false;
            AccessToken = null;
        }

        public async Task<bool> TryRefreshAsync()
        {
            await Task.Delay(200);
            return IsLoggedIn;
        }
    }
#endif
}
