namespace Champions.Services
{
    /// <summary>
    /// Platform-agnostic secure key-value store.
    ///
    /// Production implementations:
    ///   iOS  — Keychain Services (via Unity iOS native plugin or a vetted asset)
    ///   Android — Android Keystore / EncryptedSharedPreferences
    ///
    /// Mock implementation (CHAMPIONS_MOCK_SERVICES only):
    ///   PlayerPrefsSecureStorage — PlayerPrefs, NOT encrypted, DEV ONLY, never ship.
    ///
    /// Register via ServiceLocator in AppManager.Awake():
    ///   #if CHAMPIONS_MOCK_SERVICES
    ///     ServiceLocator.Register<ISecureStorage>(new PlayerPrefsSecureStorage());
    ///   #else
    ///     ServiceLocator.Register<ISecureStorage>(new NativeSecureStorage());
    ///   #endif
    ///
    /// Used by IAuthService implementations to persist refresh tokens between sessions.
    /// NEVER use it to store plaintext user passwords.
    /// </summary>
    public interface ISecureStorage
    {
        /// <summary>Write or overwrite <paramref name="value"/> at <paramref name="key"/>.</summary>
        void   Set(string key, string value);

        /// <summary>Read value, or <c>null</c> when not found.</summary>
        string Get(string key);

        /// <summary>Remove the entry if it exists.</summary>
        void   Delete(string key);
    }
}
