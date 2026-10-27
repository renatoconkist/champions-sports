using Conkist.GDK;
using UnityEngine;

namespace Champions.Systems
{
    /// <summary>
    /// Controls tab switching inside the Main scene.
    /// Tab order: Home (Dashboard), Treino, Conhecimento, Ranking, Perfil.
    /// </summary>
    public class MainSceneManager : SingletonBehaviour<MainSceneManager>
    {
        [SerializeField] private GameObject _homeTab;
        [SerializeField] private GameObject _treinoTab;
        [SerializeField] private GameObject _conhecimentoTab;
        [SerializeField] private GameObject _rankingTab;
        [SerializeField] private GameObject _perfilTab;

        public enum MainTab { Home, Treino, Conhecimento, Ranking, Perfil }

        private MainTab _currentTab = MainTab.Home;

        protected override void Awake()
        {
            persistent = false;
            base.Awake();
        }

        private void Start() => SwitchTab(MainTab.Home);

        public void SwitchTab(MainTab tab)
        {
            _currentTab = tab;
            if (_homeTab)         _homeTab.SetActive(tab         == MainTab.Home);
            if (_treinoTab)       _treinoTab.SetActive(tab       == MainTab.Treino);
            if (_conhecimentoTab) _conhecimentoTab.SetActive(tab == MainTab.Conhecimento);
            if (_rankingTab)      _rankingTab.SetActive(tab      == MainTab.Ranking);
            if (_perfilTab)       _perfilTab.SetActive(tab       == MainTab.Perfil);
        }
    }
}
