using Champions.Data.DTOs;
using Champions.Systems;
using Conkist.GDK;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UIElements;

namespace Champions.UI
{
    /// <summary>Drives the Register / Cadastro screen.</summary>
    [RequireComponent(typeof(UIDocument))]
    public class RegisterController : UIDocumentBehaviour
    {
        private TextField _firstName, _lastName, _email, _password, _confirmPassword;
        private Button    _registerButton, _backButton;
        private Label     _errorLabel;

        // ── Cached localized error texts (System table) ───────────────────
        private string _cachedPasswordMismatch = "As senhas não coincidem.";
        private string _cachedRegisterFailed   = "Não foi possível criar a conta.";

        // ── Lifecycle ─────────────────────────────────────────────────────
        protected override void OnDocumentReady()
        {
            ClearLocalizedBindings();

            var root = Document.rootVisualElement;

            _firstName       = root.Q<TextField>("first-name-field");
            _lastName        = root.Q<TextField>("last-name-field");
            _email           = root.Q<TextField>("email-field");
            _password        = root.Q<TextField>("password-field");
            _confirmPassword = root.Q<TextField>("confirm-pass-field");
            _registerButton  = root.Q<Button>("register-button");
            _backButton      = root.Q<Button>("back-button");
            _errorLabel      = root.Q<Label>("error-label");

            _registerButton?.RegisterCallback<ClickEvent>(_ => OnRegisterClicked());
            _backButton    ?.RegisterCallback<ClickEvent>(_ => OnBackClicked());

            // Only error strings need code-side caching — UI text/placeholders
            // are bound directly in the UXML via the Localization binding system.
            BindLocalized(new LocalizedString("System", "error_password_mismatch"),
                t => _cachedPasswordMismatch = t);
            BindLocalized(new LocalizedString("System", "error_register_failed"),
                t => _cachedRegisterFailed = t);
        }

        private void OnDisable() => ClearLocalizedBindings();

        // ── Action handlers ───────────────────────────────────────────────
        private async void OnRegisterClicked()
        {
            if (_errorLabel != null) _errorLabel.text = string.Empty;

            if (_password?.value != _confirmPassword?.value)
            {
                if (_errorLabel != null) _errorLabel.text = _cachedPasswordMismatch;
                return;
            }

            var req = new RegisterRequest
            {
                username = $"{_firstName?.value} {_lastName?.value}".Trim(),
                email    = _email?.value    ?? string.Empty,
                password = _password?.value ?? string.Empty
            };

            bool ok = await AuthSystem.Instance.RegisterAsync(req, destroyCancellationToken);

            if (ok)
                NavigationSystem.GoToMain();
            else if (_errorLabel != null)
                _errorLabel.text = _cachedRegisterFailed;
        }

        private void OnBackClicked() =>
            AuthSceneManager.Instance.ShowScreen(AuthSceneManager.AuthScreen.Login);
    }
}
