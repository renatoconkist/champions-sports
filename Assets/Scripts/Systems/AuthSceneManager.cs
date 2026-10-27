using Conkist.GDK;
using UnityEngine;

namespace Champions.Systems
{
    /// <summary>
    /// Controls screen transitions within the Auth scene.
    /// Manages Login / Register / ForgotPassword / Support panels on a single canvas.
    /// </summary>
    public class AuthSceneManager : SingletonBehaviour<AuthSceneManager>
    {
        [SerializeField] private GameObject _loginPanel;
        [SerializeField] private GameObject _registerPanel;
        [SerializeField] private GameObject _forgotPasswordPanel;
        [SerializeField] private GameObject _supportPanel;

        public enum AuthScreen { Login, Register, ForgotPassword, Support }

        protected override void Awake()
        {
            persistent = false;
            base.Awake();
        }

        private void Start() => ShowScreen(AuthScreen.Login);

        public void ShowScreen(AuthScreen screen)
        {
            SetActive(_loginPanel,          screen == AuthScreen.Login);
            SetActive(_registerPanel,       screen == AuthScreen.Register);
            SetActive(_forgotPasswordPanel, screen == AuthScreen.ForgotPassword);
            SetActive(_supportPanel,        screen == AuthScreen.Support);
        }

        private static void SetActive(GameObject go, bool value)
        {
            if (go != null) go.SetActive(value);
        }
    }
}
