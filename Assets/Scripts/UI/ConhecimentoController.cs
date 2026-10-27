using System.Collections.Generic;
using Champions.Data.DTOs;
using Champions.Services;
using Conkist.GDK;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Champions.UI
{
    /// <summary>
    /// Drives the Conhecimento (Knowledge) tab in the Main scene.
    /// Fetches the content bundle and renders cards grouped by category.
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class ConhecimentoController : UIDocumentBehaviour
    {
        private ScrollView _contentScroll;
        private Button     _catAll, _catRegras, _catTatica, _catHistoria;
        private Label      _challengeTitle;
        private Button     _challengeBtn;

        private ContentBundle _bundle;
        private string        _activeCategory = "all";

        protected override void OnDocumentReady()
        {
            var root = Document.rootVisualElement;
            _contentScroll  = root.Q<ScrollView>("content-scroll");
            _catAll         = root.Q<Button>("cat-all");
            _catRegras      = root.Q<Button>("cat-regras");
            _catTatica      = root.Q<Button>("cat-tatica");
            _catHistoria    = root.Q<Button>("cat-historia");
            _challengeTitle = root.Q<Label>("challenge-title");
            _challengeBtn   = root.Q<Button>("challenge-btn");

            _catAll?.RegisterCallback<ClickEvent>(_      => FilterBy("all"));
            _catRegras?.RegisterCallback<ClickEvent>(_   => FilterBy("know"));
            _catTatica?.RegisterCallback<ClickEvent>(_   => FilterBy("tatica"));
            _catHistoria?.RegisterCallback<ClickEvent>(_ => FilterBy("historia"));

            _challengeBtn?.RegisterCallback<ClickEvent>(_ => OnChallengeClicked());

            _ = LoadAsync();
        }

        private async UniTaskVoid LoadAsync()
        {
            if (_contentScroll == null) return;
            _contentScroll.Clear();
            _contentScroll.Add(MakeInfoLabel("Carregando conteúdos…"));

            try
            {
                var svc = ServiceLocator.Get<IContentService>();
                _bundle = await svc.GetContentAsync().AsUniTask()
                                   .AttachExternalCancellation(destroyCancellationToken);

                // Show featured card as "daily challenge"
                var featured = _bundle?.featured;
                if (featured != null && featured.Count > 0 && _challengeTitle != null)
                    _challengeTitle.text = featured[0].title;

                Render("all");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[ConhecimentoController] {ex.Message}");
                _contentScroll.Clear();
                _contentScroll.Add(MakeInfoLabel("Erro ao carregar conteúdo."));
            }
        }

        private void FilterBy(string category)
        {
            _activeCategory = category;
            SetCatActive(_catAll,      category == "all");
            SetCatActive(_catRegras,   category == "know");
            SetCatActive(_catTatica,   category == "tatica");
            SetCatActive(_catHistoria, category == "historia");
            Render(category);
        }

        private void Render(string category)
        {
            if (_contentScroll == null || _bundle == null) return;
            _contentScroll.Clear();

            var allCards = new List<ContentCard>();
            if (_bundle.news       != null) allCards.AddRange(_bundle.news);
            if (_bundle.events     != null) allCards.AddRange(_bundle.events);
            if (_bundle.challenges != null) allCards.AddRange(_bundle.challenges);

            var cards = category == "all"
                ? allCards
                : allCards.FindAll(c => c.category == category);

            if (cards.Count == 0) { _contentScroll.Add(MakeInfoLabel("Nenhum conteúdo encontrado.")); return; }
            foreach (var c in cards) _contentScroll.Add(MakeContentCard(c));
        }

        private VisualElement MakeContentCard(ContentCard card)
        {
            // Background colour by style tag
            Color bg = card.style switch
            {
                "blue"  => new Color(0.922f, 0.953f, 0.996f),
                "green" => new Color(0.922f, 0.996f, 0.941f),
                "gold"  => new Color(0.996f, 0.957f, 0.898f),
                _       => new Color(0.922f, 0.953f, 0.996f)
            };

            var card_ve = new VisualElement();
            card_ve.AddToClassList("cs-card");
            card_ve.style.marginBottom   = 12;
            card_ve.style.backgroundColor = new StyleColor(bg);

            var title = new Label(card.title ?? "Conteúdo");
            title.style.fontSize = 15;
            title.style.color    = new StyleColor(new Color(0.051f, 0.106f, 0.165f));
            title.style.unityFontStyleAndWeight = FontStyle.Bold;
            title.style.whiteSpace = WhiteSpace.Normal;
            title.style.marginBottom = 8;

            var readBtn = new Button(() => OnCardClicked(card)) { text = "Ler →" };
            readBtn.AddToClassList("cs-button-ghost");
            readBtn.style.height = 36;

            card_ve.Add(title);
            card_ve.Add(readBtn);
            return card_ve;
        }

        private void OnChallengeClicked()
        {
            var featured = _bundle?.featured;
            if (featured != null && featured.Count > 0)
                OnCardClicked(featured[0]);
        }

        private void OnCardClicked(ContentCard card)
        {
            Debug.Log($"[ConhecimentoController] Open content {card.id}: {card.title}");
            // TODO: navigate to content detail / video player
        }

        private static void SetCatActive(Button btn, bool active)
        {
            if (btn == null) return;
            if (active) { btn.AddToClassList("cat-chip--active"); btn.RemoveFromClassList("cat-chip"); }
            else        { btn.RemoveFromClassList("cat-chip--active"); btn.AddToClassList("cat-chip"); }
        }

        private static Label MakeInfoLabel(string msg) =>
            new(msg) { style = { color = new StyleColor(new Color(0.38f, 0.47f, 0.6f)), fontSize = 14, unityTextAlign = TextAnchor.MiddleCenter } };
    }
}
