using UnityEngine.UIElements;

namespace Champions.UI.Components
{
    /// <summary>
    /// Custom Visual Element — StatBadge.
    /// Pode ser usado diretamente em UXML como &lt;champions:StatBadge /&gt;
    /// ou instanciado via template e configurado via código.
    ///
    /// Exemplo UXML:
    ///     xmlns:champions="Champions.UI.Components"
    ///     &lt;champions:StatBadge title="Nível" value="12" /&gt;
    ///
    /// Exemplo código:
    ///     var badge = root.Q&lt;StatBadgeElement&gt;("level-badge");
    ///     badge.SetData("Nível", "12");
    /// </summary>
    public class StatBadgeElement : VisualElement
    {
        // ── UxmlFactory / UxmlTraits (permite usar em UXML com atributos) ──

        public new class UxmlFactory : UxmlFactory<StatBadgeElement, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription _value =
                new() { name = "value", defaultValue = "0" };

            private readonly UxmlStringAttributeDescription _title =
                new() { name = "title", defaultValue = "Stat" };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var el = (StatBadgeElement)ve;
                el.SetData(_title.GetValueFromBag(bag, cc), _value.GetValueFromBag(bag, cc));
            }
        }

        // ── Fields ────────────────────────────────────────────────────────

        private readonly Label _valueLabel;
        private readonly Label _titleLabel;

        // ── Constructor ───────────────────────────────────────────────────

        public StatBadgeElement()
        {
            AddToClassList("stat-badge");

            _valueLabel = new Label { name = "value-label" };
            _valueLabel.AddToClassList("stat-badge__value");

            _titleLabel = new Label { name = "title-label" };
            _titleLabel.AddToClassList("stat-badge__title");

            Add(_valueLabel);
            Add(_titleLabel);
        }

        // ── API ───────────────────────────────────────────────────────────

        public void SetData(string title, string value)
        {
            _titleLabel.text = title;
            _valueLabel.text = value;
        }

        public void SetData(string title, int value)    => SetData(title, value.ToString());
        public void SetData(string title, float value)  => SetData(title, value.ToString("F0"));
    }
}
