using UnityEngine;
using UnityEngine.UIElements;

namespace Champions.UI.Components
{
    /// <summary>
    /// Barra de progresso customizável via UXML.
    ///
    /// Uso em UXML (adicionar namespace no elemento raiz):
    ///     xmlns:champions="Champions.UI.Components"
    ///     &lt;champions:ProgressBarElement name="progress-bar" percentage="60" /&gt;
    ///
    /// Uso via código:
    ///     root.Q&lt;ProgressBarElement&gt;("progress-bar").Percentage = 75f;
    ///     root.Q&lt;ProgressBarElement&gt;("progress-bar").SetProgress(current: 360, max: 500);
    /// </summary>
    [UxmlElement]
    public partial class ProgressBarElement : VisualElement
    {
        // ── DOM interno ───────────────────────────────────────────────────

        private readonly VisualElement _background;
        private readonly VisualElement _fill;
        private float _value;

        // ── Construtor ────────────────────────────────────────────────────

        public ProgressBarElement()
        {
            // Estrutura espelha ProgressBar.uxml: #background > #fill
            _background = new VisualElement { name = "background" };
            _background.AddToClassList("progress-bar");

            _fill = new VisualElement { name = "fill" };
            _fill.AddToClassList("progress-bar__fill");

            _background.Add(_fill);
            Add(_background);

            // Aplica valor padrão (0 → fill 0%)
            ApplyFill();
        }

        // ── Atributo UXML ─────────────────────────────────────────────────

        /// <summary>
        /// Progresso de 0 a 100 (porcentagem).
        /// Editável direto no UI Builder ou como atributo UXML: percentage="75"
        /// </summary>
        [UxmlAttribute("percentage")]
        public float Percentage
        {
            get => _value * 100f;
            set
            {
                _value = Mathf.Clamp(value, 0f, 100f) / 100f;
                ApplyFill();
            }
        }

        // ── API pública ───────────────────────────────────────────────────

        /// <summary>Define progresso a partir de valores absolutos (ex: 360 XP de 500).</summary>
        public void SetProgress(float current, float max)
        {
            Percentage = max > 0f ? (current / max) * 100f : 0f;
        }

        // ── Interno ───────────────────────────────────────────────────────

        private void ApplyFill()
        {
            if (_fill == null) return;
            _fill.style.width = Length.Percent(_value * 100f);
        }
    }
}
