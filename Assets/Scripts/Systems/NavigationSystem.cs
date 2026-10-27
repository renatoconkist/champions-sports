using UnityEngine;
using UnityEngine.SceneManagement;

namespace Champions.Systems
{
    /// <summary>
    /// Champions scene navigation helper.
    /// Scene names must match the Build Settings entries exactly.
    ///
    /// GoToMain() delegates to LoadingSystem so the loading overlay is shown
    /// automatically — callers don't need to know about the transition.
    /// </summary>
    public static class NavigationSystem
    {
        private const string SceneAuth       = "Auth";
        private const string SceneOnboarding = "Onboarding";

        public static void GoToAuth()
        {
            Debug.Log("[Nav] → Auth");
            SceneManager.LoadScene(SceneAuth);
        }

        public static void GoToOnboarding()
        {
            Debug.Log("[Nav] → Onboarding");
            SceneManager.LoadScene(SceneOnboarding);
        }

        /// <summary>
        /// Transitions to Main: shows the persistent loading overlay, refreshes
        /// the profile, loads Main additively, then unloads the current scene.
        /// </summary>
        /// <param name="fromScene">Scene to unload after Main is ready (default: Auth).</param>
        public static void GoToMain(string fromScene = SceneAuth)
        {
            Debug.Log($"[Nav] → Main (from {fromScene})");
            LoadingSystem.GoToMain(fromScene);
        }
    }
}
