using Champions.Data.ViewModels;
using Champions.Systems;
using Conkist.GDK;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UIElements;

namespace Champions.UI
{
    /// <summary>
    /// Drives the Login UXML screen.
    /// Binds to LoginViewModel (set as UIDocument.dataSource in the Inspector).
    ///
    /// Localized texts come from the "Auth" and "System" String Tables via
    /// LocalizedString.StringChanged — no manual FetchString calls needed.
    ///
    /// Language picker (flag dropdown): updates CSS classes on flag buttons;
    /// still subscribes to SelectedLocaleChanged for the flag visuals because
    /// that logic is about locale codes (pt/en/es), not localized text.
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class LoginController : UIDocumentBehaviour
    {
        [SerializeField] private LoginViewModel _viewModel;

        // ── UXML element refs ─────────────────────────────────────────────
        private Button        _languageBtn;
        private VisualElement _languagePicker;
        private Button        _flagPtBtn, _flagEnBtn, _flagEsBtn;

        private TextField     _emailField, _passwordField;
        private Button        _loginButton, _registerButton, _forgotButton, _supportButton;
        private Label         _errorLabel;

        // ── Cached localized error text ───────────────────────────────────
        private string _cachedWrongCredentials = "E-mail ou senha incorretos.";

        // ── Locale change (for flag visuals only) ─────────────────────────
        private System.Action<Locale> _localeChangedHandler;

        // ── Lifecycle ─────────────────────────────────────────────────────
        protected override void OnDocumentReady()
        {
            // Unsubscribe previous text bindings before re-querying elements.
            ClearLocalizedBindings();

            var root = Document.rootVisualElement;

            // Language picker
            _languageBtn    = root.Q<Button>("language-btn");
            _languagePicker = root.Q<VisualElement>("language-picker");
            _flagPtBtn      = root.Q<Button>("flag-pt");
            _flagEnBtn      = root.Q<Button>("flag-en");
            _flagEsBtn      = root.Q<Button>("flag-es");

            // Form
            _emailField     = root.Q<TextField>("email-field");
            _passwordField  = root.Q<TextField>("password-field");
            _loginButton    = root.Q<Button>("login-button");
            _registerButton = root.Q<Button>("register-button");
            _forgotButton   = root.Q<Button>("forgot-button");
            _supportButton  = root.Q<Button>("support-button");
            _errorLabel     = root.Q<Label>("error-label");

            BindViewModel();
            RegisterCallbacks();
            BindLocalizedTexts();

            // Flag visuals depend on locale code (not a string table entry).
            RefreshMainFlag();
            RefreshPickerActiveFlag();

            if (_localeChangedHandler != null)
                LocalizationSettings.SelectedLocaleChanged -= _localeChangedHandler;
            _localeChangedHandler = _ => { RefreshMainFlag(); RefreshPickerActiveFlag(); };
            LocalizationSettings.SelectedLocaleChanged += _localeChangedHandler;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        private void OnDisable()
        {
            ClearLocalizedBindings();
            if (_localeChangedHandler != null)
            {
                LocalizationSettings.SelectedLocaleChanged -= _localeChangedHandler;
                _localeChangedHandler = null;
            }
        }

        // ── Data binding ─────────────────────────────────────────────────
        private void BindViewModel()
        {
            if (_viewModel == null) return;
            Document.rootVisualElement.dataSource = _viewModel;
        }

        // ── Localized text bindings ───────────────────────────────────────
        private void BindLocalizedTexts()
        {
            // Capture element refs — lambdas close over these locals so
            // they remain valid even if OnDocumentReady rebuilds the tree.
            var loginBtn    = _loginButton;
            var registerBtn = _registerButton;
            var forgotBtn   = _forgotButton;
            var supportBtn  = _supportButton;
            var emailFld    = _emailField;
            var passFld     = _passwordField;

            BindLocalized(new LocalizedString("Auth", "login_btn"),
                t => { if (loginBtn    != null) loginBtn.text    = t; });

            BindLocalized(new LocalizedString("Auth", "login_register_btn"),
                t => { if (registerBtn != null) registerBtn.text = t; });

            BindLocalized(new LocalizedString("Auth", "login_forgot_btn"),
                t => { if (forgotBtn   != null) forgotBtn.text   = t; });

            BindLocalized(new LocalizedString("Auth", "login_support_btn"),
                t => { if (supportBtn  != null) supportBtn.text  = t; });

            BindLocalized(new LocalizedString("Auth", "login_email_ph"),
                t => { if (emailFld != null) emailFld.textEdition.placeholder = t; });

            BindLocalized(new LocalizedString("Auth", "login_password_ph"),
                t => { if (passFld  != null) passFld.textEdition.placeholder  = t; });

            // Cache error text so it is ready when OnLoginClicked fires.
            BindLocalized(new LocalizedString("System", "error_wrong_credentials"),
                t => _cachedWrongCredentials = t);
        }

        // ── Callbacks ─────────────────────────────────────────────────────
        private void RegisterCallbacks()
        {
            _loginButton   ?.RegisterCallback<ClickEvent>(_ => OnLoginClicked());
            _registerButton?.RegisterCallback<ClickEvent>(_ => OnRegisterClicked());
            _forgotButton  ?.RegisterCallback<ClickEvent>(_ => OnForgotClicked());
            _supportButton ?.RegisterCallback<ClickEvent>(_ => OnSupportClicked());
            _languageBtn   ?.RegisterCallback<ClickEvent>(_ => TogglePicker());
            _flagPtBtn     ?.RegisterCallback<ClickEvent>(_ => SelectLocale("pt"));
            _flagEnBtn     ?.RegisterCallback<ClickEvent>(_ => SelectLocale("en"));
            _flagEsBtn     ?.RegisterCallback<ClickEvent>(_ => SelectLocale("es"));

            Document.rootVisualElement.RegisterCallback<PointerDownEvent>(
                OnRootPointerDown, TrickleDown.TrickleDown);
        }

        // ── Language picker ───────────────────────────────────────────────
        private void TogglePicker()
        {
            if (_languagePicker == null) return;
            bool isOpen = _languagePicker.style.display == DisplayStyle.Flex;
            _languagePicker.style.display = isOpen ? DisplayStyle.None : DisplayStyle.Flex;
            if (!isOpen) RefreshPickerActiveFlag();
        }

        private void ClosePicker()
        {
            if (_languagePicker != null)
                _languagePicker.style.display = DisplayStyle.None;
        }

        private void SelectLocale(string code)
        {
            ClosePicker();
            if (!LocalizationSystem.IsLocaleAvailable(code)) return;
            LocalizationSystem.SetLocale(code);
            // SelectedLocaleChanged fires → RefreshMainFlag + RefreshPickerActiveFlag
        }

        private void OnRootPointerDown(PointerDownEvent evt)
        {
            if (_languagePicker == null) return;
            if (_languagePicker.style.display != DisplayStyle.Flex) return;
            bool hitPicker = _languagePicker.worldBound.Contains(evt.position);
            bool hitBtn    = _languageBtn?.worldBound.Contains(evt.position) ?? false;
            if (!hitPicker && !hitBtn) ClosePicker();
        }

        // ── Flag visuals ──────────────────────────────────────────────────
        private static readonly string[] FlagClasses =
            { "cs-button-language--pt", "cs-button-language--en", "cs-button-language--es" };

        private void RefreshMainFlag()
        {
            if (_languageBtn == null) return;
            var code = LocalizationSettings.SelectedLocale?.Identifier.Code ?? "";
            string flagClass;
            if      (code.StartsWith("en")) flagClass = "cs-button-language--en";
            else if (code.StartsWith("es")) flagClass = "cs-button-language--es";
            else                            flagClass = "cs-button-language--pt";

            foreach (var cls in FlagClasses) _languageBtn.RemoveFromClassList(cls);
            _languageBtn.AddToClassList(flagClass);
        }

        private void RefreshPickerActiveFlag()
        {
            var code = LocalizationSettings.SelectedLocale?.Identifier.Code ?? "pt";
            SetFlagActive(_flagPtBtn, code.StartsWith("pt"));
            SetFlagActive(_flagEnBtn, code.StartsWith("en"));
            SetFlagActive(_flagEsBtn, code.StartsWith("es"));
        }

        private static void SetFlagActive(Button btn, bool active)
        {
            if (btn == null) return;
            if (active) btn.AddToClassList("language-picker__flag--active");
            else        btn.RemoveFromClassList("language-picker__flag--active");
        }

        // ── Action handlers ───────────────────────────────────────────────
        private async void OnLoginClicked()
        {
            if (_viewModel == null) return;
            _viewModel.IsLoading    = true;
            _viewModel.ErrorMessage = string.Empty;

            bool ok = await AuthSystem.Instance.LoginAsync(
                _viewModel.Email, _viewModel.Password, destroyCancellationToken);

            _viewModel.IsLoading = false;
            if (!ok) _viewModel.ErrorMessage = _cachedWrongCredentials;
            else     NavigationSystem.GoToMain();
        }

        private void OnRegisterClicked() =>
            AuthSceneManager.Instance.ShowScreen(AuthSceneManager.AuthScreen.Register);

        private void OnForgotClicked() =>
            AuthSceneManager.Instance.ShowScreen(AuthSceneManager.AuthScreen.ForgotPassword);

        private void OnSupportClicked() =>
            AuthSceneManager.Instance.ShowScreen(AuthSceneManager.AuthScreen.Support);
    }
}
