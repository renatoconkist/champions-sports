#if CHAMPIONS_MOCK_SERVICES
using UnityEngine;

namespace Champions.Services.Mock
{
    /// <summary>
    /// ⚠️  DEV / MOCK ONLY — NOT ENCRYPTED — NEVER SHIP.
    ///
    /// Backs ISecureStorage with PlayerPrefs so the mock auth flow persists a fake
    /// refresh token between Editor Play sessions.
    ///
    /// Replace with NativeSecureStorage (iOS Keychain / Android Keystore) before
    /// building a release. See docs/ARCHITECTURE.md → Secure storage handoff.
    /// </summary>
    public class PlayerPrefsSecureStorage : ISecureStorage
    {
        private const string Prefix = "sec_";   // namespace to avoid key collisions

        public void Set(string key, string value)
        {
            PlayerPrefs.SetString(Prefix + key, value);
            PlayerPrefs.Save();
        }

        public string Get(string key) =>
            PlayerPrefs.GetString(Prefix + key, null);

        public void Delete(string key)
        {
            PlayerPrefs.DeleteKey(Prefix + key);
            PlayerPrefs.Save();
        }
    }
}
#endif
