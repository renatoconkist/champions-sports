using Conkist.GDK;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Champions.Systems
{
    /// <summary>
    /// Runs on app start (Boot scene).
    /// Initialises services, attempts session restore, then routes to Auth or Main.
    ///
    /// Auto-login relies entirely on the backend refresh token stored through
    /// ISecureStorage — no plaintext credentials are ever persisted.
    /// See docs/ARCHITECTURE.md → Secure storage.
    ///
    /// Attach to _BootManager in Boot.unity.
    /// </summary>
    public class BootSystem : SingletonBehaviour<BootSystem>
    {
        protected override void Awake()
        {
            persistent = false;
            base.Awake();
        }

        private void Start() =>
            RunAsync().Forget(ex => Debug.LogException(ex));

        private async UniTask RunAsync()
        {
            // Small delay so the splash can render before async work begins.
            await UniTask.Delay(300);

            // Try to restore the previous session via refresh token (silent).
            bool restored = await AuthSystem.Instance.TryRestoreSessionAsync(destroyCancellationToken);

            if (restored)
            {
                RouteAuthenticatedUser();
                return;
            }

            // Refresh token missing or expired — user must log in manually.
            NavigationSystem.GoToAuth();
        }

        private void RouteAuthenticatedUser()
        {
            bool onboardingDone = PlayerPrefs.GetInt("onboarding_done", 0) == 1;
            if (onboardingDone)
                NavigationSystem.GoToMain("Boot");
            else
                NavigationSystem.GoToOnboarding();
        }
    }
}
