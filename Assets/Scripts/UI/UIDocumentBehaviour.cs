using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UIElements;

namespace Champions.UI
{
    /// <summary>
    /// Base MonoBehaviour for UI Toolkit screens.
    ///
    /// — Re-calls OnDocumentReady() on every re-enable because Unity UI Toolkit
    ///   rebuilds the visual tree each time a UIDocument is re-enabled.
    ///
    /// — Provides BindLocalized() to wire LocalizedString.StringChanged events
    ///   directly to UI element properties. Call ClearLocalizedBindings() at
    ///   the top of OnDocumentReady (before re-binding) and in OnDisable.
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public abstract class UIDocumentBehaviour : MonoBehaviour
    {
        private UIDocument _document;
        private bool _started;

        private readonly List<(LocalizedString ls, LocalizedString.ChangeHandler handler)>
            _locBindings = new();

        /// <summary>The UIDocument on this GameObject.</summary>
        protected UIDocument Document
        {
            get
            {
                if (_document == null)
                    _document = GetComponent<UIDocument>();
                return _document;
            }
        }

        protected virtual void Start()
        {
            _started = true;
            OnDocumentReady();
        }

        protected virtual void OnEnable()
        {
            if (_started) OnDocumentReady();
        }

        /// <summary>
        /// Called on Start and on every subsequent re-enable.
        /// Override to query elements, bind data and register callbacks.
        /// Call ClearLocalizedBindings() at the top before re-binding.
        /// </summary>
        protected virtual void OnDocumentReady() { }

        // ── LocalizedString binding helpers ───────────────────────────────

        /// <summary>
        /// Subscribes <paramref name="onChanged"/> to
        /// <paramref name="ls"/>.StringChanged and tracks it for later cleanup.
        /// StringChanged fires immediately with the current locale value and
        /// again whenever the locale changes.
        /// </summary>
        protected void BindLocalized(LocalizedString ls,
                                     LocalizedString.ChangeHandler onChanged)
        {
            ls.StringChanged += onChanged;
            _locBindings.Add((ls, onChanged));
        }

        /// <summary>
        /// Unsubscribes all handlers registered via BindLocalized().
        /// Call at the start of OnDocumentReady and in OnDisable.
        /// </summary>
        protected void ClearLocalizedBindings()
        {
            foreach (var (ls, handler) in _locBindings)
                ls.StringChanged -= handler;
            _locBindings.Clear();
        }
    }
}
