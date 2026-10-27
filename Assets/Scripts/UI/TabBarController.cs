using Champions.Systems;
using Conkist.GDK;
using UnityEngine;
using UnityEngine.UIElements;

namespace Champions.UI
{
    /// <summary>
    /// Drives the bottom tab bar in the Main scene.
    /// Syncs button active states and delegates tab switching to MainSceneManager.
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class TabBarController : UIDocumentBehaviour
    {
        private Button _homeBtn, _treinoBtn, _conhecimentoBtn, _rankingBtn, _perfilBtn;

        protected override void OnDocumentReady()
        {
            var root = Document.rootVisualElement;
            _homeBtn         = root.Q<Button>("tab-home");
            _treinoBtn       = root.Q<Button>("tab-treino");
            _conhecimentoBtn = root.Q<Button>("tab-conhecimento");
            _rankingBtn      = root.Q<Button>("tab-ranking");
            _perfilBtn       = root.Q<Button>("tab-perfil");

            _homeBtn?.RegisterCallback<ClickEvent>(_ => Switch(MainSceneManager.MainTab.Home));
            _treinoBtn?.RegisterCallback<ClickEvent>(_ => Switch(MainSceneManager.MainTab.Treino));
            _conhecimentoBtn?.RegisterCallback<ClickEvent>(_ => Switch(MainSceneManager.MainTab.Conhecimento));
            _rankingBtn?.RegisterCallback<ClickEvent>(_ => Switch(MainSceneManager.MainTab.Ranking));
            _perfilBtn?.RegisterCallback<ClickEvent>(_ => Switch(MainSceneManager.MainTab.Perfil));

            SetActiveButton(MainSceneManager.MainTab.Home);
        }

        private void Switch(MainSceneManager.MainTab tab)
        {
            MainSceneManager.Instance.SwitchTab(tab);
            SetActiveButton(tab);
        }

        private void SetActiveButton(MainSceneManager.MainTab active)
        {
            SetActive(_homeBtn,         active == MainSceneManager.MainTab.Home);
            SetActive(_treinoBtn,       active == MainSceneManager.MainTab.Treino);
            SetActive(_conhecimentoBtn, active == MainSceneManager.MainTab.Conhecimento);
            SetActive(_rankingBtn,      active == MainSceneManager.MainTab.Ranking);
            SetActive(_perfilBtn,       active == MainSceneManager.MainTab.Perfil);
        }

        private static void SetActive(Button btn, bool isActive)
        {
            if (btn == null) return;
            if (isActive) btn.AddToClassList("tab--active");
            else          btn.RemoveFromClassList("tab--active");
        }
    }
}
