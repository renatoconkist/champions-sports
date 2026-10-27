using Conkist.GDK;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using USceneManager = UnityEngine.SceneManagement.SceneManager;
using ULoadSceneMode = UnityEngine.SceneManagement.LoadSceneMode;

namespace Champions.Systems
{
    /// <summary>
    /// Persistent loading overlay — loaded once at boot, never destroyed.
    ///
    /// Static API (call from anywhere, even before the instance is fully ready):
    ///   LoadingSystem.Show();
    ///   LoadingSystem.Hide();
    ///   LoadingSystem.GoToMain();
    /// </summary>
    public class LoadingSystem : SingletonBehaviour<LoadingSystem>
    {
        private const string SceneMain = "Main";

        // The UIDocument lives on a child GameObject (LoadingUI) — not on this one.
        private GameObject _overlayGO;

        // ── Lifecycle ─────────────────────────────────────────────────────────

        protected override void Awake()
        {
            persistent = true;          // DontDestroyOnLoad
            base.Awake();
            // Search children (including inactive) for the UIDocument holder.
            var doc = GetComponentInChildren<UIDocument>(includeInactive: true);
            _overlayGO = doc != null ? doc.gameObject : null;
            HideOverlay();
        }

        // ── Static API ────────────────────────────────────────────────────────

        /// <summary>Show the loading overlay. Safe to call before the scene finishes loading.</summary>
        public static void Show() => Instance?.ShowOverlay();

        /// <summary>Hide the loading overlay.</summary>
        public static void Hide() => Instance?.HideOverlay();

        /// <summary>
        /// Shows the overlay, refreshes the player profile, loads Main additively,
        /// sets Main as the active scene, unloads <paramref name="fromScene"/>,
        /// then hides the overlay.
        /// </summary>
        public static void GoToMain(string fromScene = "Auth") =>
            Instance?.TransitionAsync(fromScene).Forget(ex => Debug.LogException(ex));

        // ── Instance methods ──────────────────────────────────────────────────

        private void ShowOverlay() => _overlayGO?.SetActive(true);

        private void HideOverlay() => _overlayGO?.SetActive(false);

        private async UniTask TransitionAsync(string fromScene)
        {
            ShowOverlay();

            await ProfileSystem.Instance.RefreshAsync(destroyCancellationToken);

            await USceneManager.LoadSceneAsync(SceneMain, ULoadSceneMode.Additive)
                               .ToUniTask(cancellationToken: destroyCancellationToken);

            USceneManager.SetActiveScene(USceneManager.GetSceneByName(SceneMain));

            var prev = USceneManager.GetSceneByName(fromScene);
            if (prev.IsValid())
                await USceneManager.UnloadSceneAsync(prev)
                                   .ToUniTask(cancellationToken: destroyCancellationToken);

            HideOverlay();
        }
    }
}
