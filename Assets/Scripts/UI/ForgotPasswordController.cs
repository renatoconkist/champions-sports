using Champions.Systems;
using Conkist.GDK;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UIElements;

namespace Champions.UI
{
    /// <summary>Drives the Esqueci Senha screen.</summary>
    [RequireComponent(typeof(UIDocument))]
    public class ForgotPasswordController : UIDocumentBehaviour
    {
        private Label     _titleLabel, _subtitleLabel, _errorLabel, _successLabel;
        private TextField _emailField;
        private Button    _sendButton, _backButton;

        // ── Cached localized feedback texts ───────────────────────────────
        private string _cachedSendFailed    = "Não foi possível enviar o e-mail.";
        private string _cachedRecoverySent  = "Link de recuperação enviado!";

        // ── Lifecycle ─────────────────────────────────────────────────────
        protected override void OnDocumentReady()
        {
            ClearLocalizedBindings();

            var root = Document.rootVisualElement;

            _titleLabel    = root.Q<Label>("title");
            _subtitleLabel = root.Q<Label>("subtitle");
            _emailField    = root.Q<TextField>("email-field");
            _sendButton    = root.Q<Button>("send-button");
            _backButton    = root.Q<Button>("back-button");
            _errorLabel    = root.Q<Label>("error-label");
            _successLabel  = root.Q<Label>("success-label");

            _sendButton?.RegisterCallback<ClickEvent>(_ => OnSendClicked());
            _backButton?.RegisterCallback<ClickEvent>(_ => OnBackClicked());

            BindLocalizedTexts();
        }

        private void OnDisable() => ClearLocalizedBindings();

        // ── Localized text bindings ───────────────────────────────────────
        private void BindLocalizedTexts()
        {
            var title    = _titleLabel;
            var subtitle = _subtitleLabel;
            var email    = _emailField;
            var sendBtn  = _sendButton;
            var backBtn  = _backButton;

            // Cache feedback texts for use in OnSendClicked.
            BindLocalized(new LocalizedString("System", "error_send_failed"),
                t => _cachedSendFailed   = t);

            BindLocalized(new LocalizedString("System", "success_recovery_sent"),
                t => _cachedRecoverySent = t);
        }

        // ── Action handlers ───────────────────────────────────────────────
        private async void OnSendClicked()
        {
            if (_errorLabel   != null) _errorLabel.text   = string.Empty;
            if (_successLabel != null) _successLabel.text = string.Empty;

            bool ok = await AuthSystem.Instance.ForgotPasswordAsync(
                _emailField?.value ?? string.Empty, destroyCancellationToken);

            if (ok)
            {
                if (_successLabel != null) _successLabel.text = _cachedRecoverySent;
            }
            else
            {
                if (_errorLabel != null) _errorLabel.text = _cachedSendFailed;
            }
        }

        private void OnBackClicked() =>
            AuthSceneManager.Instance.ShowScreen(AuthSceneManager.AuthScreen.Login);
    }
}
