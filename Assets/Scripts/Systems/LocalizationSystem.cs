using Conkist.GDK;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace Champions.Systems
{
    /// <summary>
    /// Thin wrapper around Unity Localization.
    ///
    /// Default locale priority on startup (highest to lowest):
    ///   1. Locale explicitly saved by the user via SetLocale() → PlayerPrefs key
    ///   2. System locale chosen by Unity's built-in StartupLocaleSelector chain
    ///      (SystemLocaleSelector → ProjectLocale).
    /// </summary>
    public static class LocalizationSystem
    {
        private const string PrefKey = "champions_locale";

        // ── Startup restore ───────────────────────────────────────────────

        /// <summary>
        /// Auto-restores the saved locale after the first scene loads.
        /// When nothing is saved, Unity's built-in locale selector picks the
        /// system language — we don't override it.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void OnAfterSceneLoad()
        {
            var code = GetSavedLocaleCode();
            if (string.IsNullOrEmpty(code)) return; // no preference → let system locale win

            // Wait for Localization to finish initializing before applying the override
            var init = LocalizationSettings.InitializationOperation;
            init.Completed += _ => RestoreSavedLocale();
        }

        // ── Public API ────────────────────────────────────────────────────

        /// <summary>Returns true when the locale is registered in Project Settings → Localization.</summary>
        public static bool IsLocaleAvailable(string localeCode) =>
            LocalizationSettings.AvailableLocales.GetLocale(localeCode) != null;

        public static void SetLocale(string localeCode)
        {
            var locale = LocalizationSettings.AvailableLocales.GetLocale(localeCode);
            if (locale == null)
            {
                Debug.LogWarning($"[LocalizationSystem] Locale '{localeCode}' not found. " +
                                 "Add it in Project Settings → Localization → Specific Locale Selector.");
                return;
            }

            LocalizationSettings.SelectedLocale = locale;
            PlayerPrefs.SetString(PrefKey, localeCode);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Returns the locale code explicitly saved by the user, or null if the
        /// user has never changed the language (system locale should be used instead).
        /// </summary>
        public static string GetSavedLocaleCode() =>
            PlayerPrefs.HasKey(PrefKey) ? PlayerPrefs.GetString(PrefKey) : null;

        /// <summary>Applies the persisted locale. No-op when nothing is saved.</summary>
        public static void RestoreSavedLocale()
        {
            var code = GetSavedLocaleCode();
            if (string.IsNullOrEmpty(code)) return;
            var locale = LocalizationSettings.AvailableLocales.GetLocale(code);
            if (locale != null)
                LocalizationSettings.SelectedLocale = locale;
        }
    }
}
