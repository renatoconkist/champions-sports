using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Champions.UI.Components
{
    /// <summary>
    /// Botão de reel individual — exibe thumbnail e número.
    ///
    /// Uso em UXML:
    ///     xmlns:champions="Champions.UI.Components"
    ///     &lt;champions:ReelsButtonElement number="3" /&gt;
    ///
    /// Uso via código (instanciação pela lista):
    ///     var btn = new ReelsButtonElement(number: 3, onClick: () => OpenReel(3));
    /// </summary>
    [UxmlElement]
    public partial class ReelsButtonElement : VisualElement
    {
        // ── DOM ───────────────────────────────────────────────────────────

        private readonly VisualElement _thumbnail;
        private readonly Label         _numberLabel;
        private int _number;

        // ── Evento ────────────────────────────────────────────────────────

        public event Action<int> Clicked;

        // ── Construtor ────────────────────────────────────────────────────

        public ReelsButtonElement()
        {
            AddToClassList("reels-btn");
            pickingMode = PickingMode.Position;

            _thumbnail = new VisualElement { name = "reels-thumbnail" };
            _thumbnail.AddToClassList("reels-btn__thumbnail");

            _numberLabel = new Label { name = "reels-number" };
            _numberLabel.AddToClassList("reels-btn__number");

            Add(_thumbnail);
            Add(_numberLabel);

            RegisterCallback<ClickEvent>(_ => Clicked?.Invoke(_number));
        }

        /// <summary>Construtor de conveniência para instanciação por código.</summary>
        public ReelsButtonElement(int number, Action<int> onClick = null) : this()
        {
            Number = number;
            if (onClick != null) Clicked += onClick;
        }

        // ── Atributo UXML ─────────────────────────────────────────────────

        /// <summary>Número do reel exibido sobre a thumbnail.</summary>
        [UxmlAttribute("number")]
        public int Number
        {
            get => _number;
            set
            {
                _number = value;
                if (_numberLabel != null)
                    _numberLabel.text = value.ToString();
            }
        }

        // ── API ───────────────────────────────────────────────────────────

        /// <summary>Define a imagem de fundo da thumbnail.</summary>
        public void SetThumbnail(Sprite sprite)
        {
            if (_thumbnail == null || sprite == null) return;
            _thumbnail.style.backgroundImage = new StyleBackground(sprite);
        }

        /// <summary>Alterna estado ativo (ex: reel selecionado).</summary>
        public void SetActive(bool active)
        {
            if (active) AddToClassList("reels-btn--active");
            else        RemoveFromClassList("reels-btn--active");
        }
    }
}
