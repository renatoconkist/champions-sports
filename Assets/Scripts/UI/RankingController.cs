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
    /// Drives the Ranking tab in the Main scene.
    /// Fetches weekly progress to place the player and displays the leaderboard.
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class RankingController : UIDocumentBehaviour
    {
        private ScrollView _rankingScroll;
        private Button     _scopeGlobal, _scopeFriends;
        private Label      _myRankPos, _myName, _myScore;

        protected override void OnDocumentReady()
        {
            var root = Document.rootVisualElement;
            _rankingScroll = root.Q<ScrollView>("ranking-scroll");
            _scopeGlobal   = root.Q<Button>("scope-global");
            _scopeFriends  = root.Q<Button>("scope-friends");
            _myRankPos     = root.Q<Label>("my-rank-pos");
            _myName        = root.Q<Label>("my-name");
            _myScore       = root.Q<Label>("my-score");

            _scopeGlobal?.RegisterCallback<ClickEvent>(_  => SetScope(global: true));
            _scopeFriends?.RegisterCallback<ClickEvent>(_ => SetScope(global: false));

            SetScopeVisual(global: true);
            _ = LoadAsync();
        }

        private async UniTaskVoid LoadAsync()
        {
            if (_rankingScroll == null) return;
            _rankingScroll.Clear();
            _rankingScroll.Add(MakeInfoLabel("Carregando ranking…"));

            try
            {
                var svc      = ServiceLocator.Get<IContentService>();
                var progress = await svc.GetWeeklyProgressAsync().AsUniTask()
                                        .AttachExternalCancellation(destroyCancellationToken);

                // Use weekly score to populate "my" card (real ranking endpoint TODO)
                if (progress != null)
                {
                    int scorePts = Mathf.RoundToInt(progress.score * 100f);
                    if (_myScore != null) _myScore.text = $"{scorePts} pts";
                }

                // Show placeholder leaderboard until real endpoint is wired
                RenderPlaceholder();
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[RankingController] {ex.Message}");
                _rankingScroll.Clear();
                _rankingScroll.Add(MakeInfoLabel("Erro ao carregar ranking."));
            }
        }

        private void SetScope(bool global)
        {
            SetScopeVisual(global);
            // TODO: fetch global vs friends leaderboard when endpoint is available
            RenderPlaceholder();
        }

        private void SetScopeVisual(bool global)
        {
            if (_scopeGlobal  != null) _scopeGlobal.style.backgroundColor  = global ? new StyleColor(UnityEngine.Color.white) : new StyleColor(UnityEngine.Color.clear);
            if (_scopeFriends != null) _scopeFriends.style.backgroundColor = global ? new StyleColor(UnityEngine.Color.clear) : new StyleColor(UnityEngine.Color.white);
        }

        /// <summary>Renders a hardcoded stub until the ranking endpoint is added to IContentService.</summary>
        private void RenderPlaceholder()
        {
            if (_rankingScroll == null) return;
            _rankingScroll.Clear();

            var entries = new (string emoji, string name, int pts)[]
            {
                ("🥇", "Rafael Costa",   980),
                ("🥈", "Lucas Mendes",   860),
                ("🥉", "Ana Souza",      790),
                ("4.", "Bruna Lima",     710),
                ("5.", "Pedro Alves",    650),
            };

            foreach (var (emoji, name, pts) in entries)
                _rankingScroll.Add(MakeRankRow(emoji, name, pts));
        }

        // ── Card builders ─────────────────────────────────────────────────────

        private static VisualElement MakeRankRow(string pos, string name, int pts)
        {
            var row = new VisualElement();
            row.AddToClassList("cs-card");
            row.style.flexDirection = FlexDirection.Row;
            row.style.alignItems    = Align.Center;
            row.style.marginBottom  = 8;

            var posLbl  = new Label(pos)  { style = { fontSize = 18, minWidth = 40, unityTextAlign = TextAnchor.MiddleCenter } };
            var nameLbl = new Label(name) { style = { fontSize = 15, flexGrow = 1, color = new StyleColor(new Color(0.051f, 0.106f, 0.165f)), unityFontStyleAndWeight = FontStyle.Bold } };
            var ptsLbl  = new Label($"{pts} pts") { style = { fontSize = 14, color = new StyleColor(new Color(0.38f, 0.47f, 0.6f)) } };

            row.Add(posLbl);
            row.Add(nameLbl);
            row.Add(ptsLbl);
            return row;
        }

        private static Label MakeInfoLabel(string msg) =>
            new(msg) { style = { color = new StyleColor(new Color(0.38f, 0.47f, 0.6f)), fontSize = 14, unityTextAlign = TextAnchor.MiddleCenter } };
    }
}
