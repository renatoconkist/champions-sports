using System.Threading;
using Champions.Data.DTOs;
using Champions.Services;
using Conkist.GDK;
// ServiceLocator is in Champions.Services — already imported above
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Champions.Systems
{
    /// <summary>
    /// Pure singleton — handles authentication flow.
    /// Owned by AppManager; survives scene changes via PureSingleton pattern.
    /// </summary>
    public class AuthSystem : PureSingleton<AuthSystem>
    {
        private IAuthService _auth;

        public bool IsAuthenticated => _auth?.IsLoggedIn ?? false;

        public AuthSystem()
        {
            _auth = ServiceLocator.Get<IAuthService>();
        }

        /// <summary>Attempt login and store token on success.</summary>
        public async UniTask<bool> LoginAsync(string email, string password, CancellationToken ct = default)
        {
            try
            {
                var req      = new LoginRequest { login = email, password = password };
                var response = await _auth.LoginAsync(req).AsUniTask().AttachExternalCancellation(ct);
                return response != null;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[AuthSystem] Login failed: {ex.Message}");
                return false;
            }
        }

        /// <summary>Register a new account and log in on success.</summary>
        public async UniTask<bool> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
        {
            try
            {
                var response = await _auth.RegisterAsync(request).AsUniTask().AttachExternalCancellation(ct);
                return response != null;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[AuthSystem] Register failed: {ex.Message}");
                return false;
            }
        }

        /// <summary>Send a password-reset email.</summary>
        public async UniTask<bool> ForgotPasswordAsync(string email, CancellationToken ct = default)
        {
            try
            {
                await _auth.ForgotPasswordAsync(new ForgotPasswordRequest { email = email })
                           .AsUniTask().AttachExternalCancellation(ct);
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[AuthSystem] ForgotPassword failed: {ex.Message}");
                return false;
            }
        }

        /// <summary>Clear session and return to Auth state.</summary>
        public async UniTask LogoutAsync(CancellationToken ct = default)
        {
            await _auth.LogoutAsync().AsUniTask().AttachExternalCancellation(ct);
        }

        /// <summary>Try to restore a previous session.</summary>
        public async UniTask<bool> TryRestoreSessionAsync(CancellationToken ct = default)
        {
            try
            {
                return await _auth.TryRefreshAsync().AsUniTask().AttachExternalCancellation(ct);
            }
            catch
            {
                return false;
            }
        }
    }
}
