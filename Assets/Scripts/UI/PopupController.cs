using System;
using UnityEngine;
using UnityEngine.UIElements;
using Conkist.GDK;

namespace Champions.UI
{
    /// <summary>
    /// Popup de alerta genérico. Singleton — persiste durante toda a sessão.
    ///
    /// Uso:
    ///   PopupController.Instance.Show("AVISO!", "Sua sessão expirou.");
    ///   PopupController.Instance.Show("AVISO!", "Deseja sair?", onConfirm: () => Logout());
    ///
    /// Setup na cena:
    ///   - Adicione um GameObject com UIDocument e este script.
    ///   - PanelSettings: Sort Order maior que todas as outras telas (ex: 10).
    ///   - Marque o GameObject como DontDestroyOnLoad se usar múltiplas cenas.
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class PopupController : UIDocumentBehaviour
    {
        public static PopupController Instance { get; private set; }

        private VisualElement _root;
        private Label         _titleLabel;
        private Label         _bodyLabel;
        private Button        _confirmButton;
        private Action        _onConfirm;

        protected override void OnDocumentReady()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            var root = Document.rootVisualElement;
            _root          = root.Q("popup-root");
            _titleLabel    = root.Q<Label>("popup-title");
            _bodyLabel     = root.Q<Label>("popup-body");
            _confirmButton = root.Q<Button>("popup-confirm");

            _confirmButton?.RegisterCallback<ClickEvent>(_ => Hide());

            // Começa oculto
            SetVisible(false);
        }

        // ── API pública ───────────────────────────────────────────────

        /// <summary>Exibe o popup com título e mensagem.</summary>
        public void Show(string title, string body, Action onConfirm = null)
        {
            if (_titleLabel != null)    _titleLabel.text = title;
            if (_bodyLabel  != null)    _bodyLabel.text  = body;
            if (_confirmButton != null) _confirmButton.text = "OK";

            _onConfirm = onConfirm;
            SetVisible(true);
        }

        /// <summary>Exibe o popup com texto personalizado no botão.</summary>
        public void Show(string title, string body, string confirmLabel, Action onConfirm = null)
        {
            Show(title, body, onConfirm);
            if (_confirmButton != null) _confirmButton.text = confirmLabel;
        }

        // ── Interno ───────────────────────────────────────────────────

        private void Hide()
        {
            SetVisible(false);
            var cb = _onConfirm;
            _onConfirm = null;
            cb?.Invoke();
        }

        private void SetVisible(bool visible)
        {
            if (_root == null) return;
            _root.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}
