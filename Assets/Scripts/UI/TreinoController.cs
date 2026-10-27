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
    /// Drives the Treino (Training) tab in the Main scene.
    /// Fetches available trainings and injects cards into the scroll view.
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class TreinoController : UIDocumentBehaviour
    {
        private ScrollView _contentScroll;
        private Button     _catAll, _catTecnica, _catTatica, _catFisico;
        private Button     _filterButton;

        private List<Training> _allTrainings = new();
        private string         _activeCategory = "all";

        protected override void OnDocumentReady()
        {
            var root = Document.rootVisualElement;
            _contentScroll = root.Q<ScrollView>("content-scroll");
            _filterButton  = root.Q<Button>("filter-button");
            _catAll        = root.Q<Button>("cat-all");
            _catTecnica    = root.Q<Button>("cat-tecnica");
            _catTatica     = root.Q<Button>("cat-tatica");
            _catFisico     = root.Q<Button>("cat-fisico");

            _catAll?.RegisterCallback<ClickEvent>(_     => FilterBy("all"));
            _catTecnica?.RegisterCallback<ClickEvent>(_ => FilterBy("tecnica"));
            _catTatica?.RegisterCallback<ClickEvent>(_  => FilterBy("tatica"));
            _catFisico?.RegisterCallback<ClickEvent>(_  => FilterBy("fisico"));

            _ = LoadAsync();
        }

        private async UniTaskVoid LoadAsync()
        {
            if (_contentScroll == null) return;
            _contentScroll.Clear();
            _contentScroll.Add(MakeLoadingLabel());

            try
            {
                var svc = ServiceLocator.Get<IContentService>();
                _allTrainings = await svc.GetTrainingsAsync().AsUniTask()
                                         .AttachExternalCancellation(destroyCancellationToken);
                Render(_allTrainings);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[TreinoController] {ex.Message}");
                _contentScroll.Clear();
                _contentScroll.Add(MakeErrorLabel("Erro ao carregar treinos."));
            }
        }

        private void FilterBy(string category)
        {
            _activeCategory = category;

            SetCatActive(_catAll,     category == "all");
            SetCatActive(_catTecnica, category == "tecnica");
            SetCatActive(_catTatica,  category == "tatica");
            SetCatActive(_catFisico,  category == "fisico");

            var filtered = category == "all"
                ? _allTrainings
                : _allTrainings.FindAll(t => t.category == category);

            Render(filtered);
        }

        private void Render(List<Training> trainings)
        {
            if (_contentScroll == null) return;
            _contentScroll.Clear();

            if (trainings == null || trainings.Count == 0)
            {
                _contentScroll.Add(MakeErrorLabel("Nenhum treino encontrado."));
                return;
            }

            foreach (var t in trainings)
                _contentScroll.Add(MakeTrainingCard(t));
        }

        // ── Card builder ─────────────────────────────────────────────────────

        private VisualElement MakeTrainingCard(Training training)
        {
            var card = new VisualElement();
            card.AddToClassList("cs-card");
            card.style.marginBottom = 12;
            card.style.flexDirection = FlexDirection.Row;
            card.style.alignItems = Align.Center;

            // Level badge
            var badge = new Label(training.level ?? "—");
            badge.style.backgroundColor = new StyleColor(new Color(0.24f, 0.75f, 0.52f, 0.2f));
            badge.style.color = new StyleColor(new Color(0.24f, 0.75f, 0.24f));
            badge.style.borderTopRightRadius = badge.style.borderTopLeftRadius =
            badge.style.borderBottomLeftRadius = badge.style.borderBottomRightRadius = 999;
            badge.style.paddingLeft = badge.style.paddingRight = 10;
            badge.style.paddingTop = badge.style.paddingBottom = 3;
            badge.style.fontSize = 11;
            badge.style.unityFontStyleAndWeight = FontStyle.Bold;
            badge.style.marginRight = 12;

            // Title
            var title = new Label(training.title ?? "Treino");
            title.style.fontSize = 15;
            title.style.color = new StyleColor(new Color(0.051f, 0.106f, 0.165f));
            title.style.unityFontStyleAndWeight = FontStyle.Bold;
            title.style.flexGrow = 1;
            title.style.whiteSpace = WhiteSpace.Normal;

            // Start button
            var btn = new Button(() => OnStartTraining(training)) { text = "▶" };
            btn.style.backgroundColor = new StyleColor(new Color(0.392f, 0.882f, 0.522f));
            btn.style.borderTopWidth = btn.style.borderBottomWidth =
            btn.style.borderLeftWidth = btn.style.borderRightWidth = 0;
            btn.style.borderTopRightRadius = btn.style.borderTopLeftRadius =
            btn.style.borderBottomLeftRadius = btn.style.borderBottomRightRadius = 999;
            btn.style.color = new StyleColor(new Color(0.051f, 0.251f, 0.125f));
            btn.style.width = btn.style.height = 36;
            btn.style.fontSize = 14;

            card.Add(badge);
            card.Add(title);
            card.Add(btn);
            return card;
        }

        private async void OnStartTraining(Training training)
        {
            Debug.Log($"[TreinoController] Starting training {training.id}: {training.title}");
            var svc = ServiceLocator.Get<IContentService>();
            await svc.StartTrainingAsync(training.id).AsUniTask()
                     .AttachExternalCancellation(destroyCancellationToken);
            // TODO: navigate to training detail / video player scene
        }

        // ── Utility ──────────────────────────────────────────────────────────

        private static void SetCatActive(Button btn, bool active)
        {
            if (btn == null) return;
            if (active) { btn.AddToClassList("cat-chip--active"); btn.RemoveFromClassList("cat-chip"); }
            else        { btn.RemoveFromClassList("cat-chip--active"); btn.AddToClassList("cat-chip"); }
        }

        private static Label MakeLoadingLabel() =>
            new("Carregando treinos…") { style = { color = new StyleColor(new Color(0.38f, 0.47f, 0.6f)), fontSize = 14, unityTextAlign = TextAnchor.MiddleCenter } };

        private static Label MakeErrorLabel(string msg) =>
            new(msg) { style = { color = new StyleColor(new Color(0.88f, 0.29f, 0.29f)), fontSize = 14, unityTextAlign = TextAnchor.MiddleCenter } };
    }
}
