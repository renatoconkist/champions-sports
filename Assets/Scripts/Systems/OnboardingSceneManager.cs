using Conkist.GDK;
using UnityEngine;

namespace Champions.Systems
{
    /// <summary>
    /// Controls the two-step Onboarding flow: Profile → Positions.
    /// </summary>
    public class OnboardingSceneManager : SingletonBehaviour<OnboardingSceneManager>
    {
        [SerializeField] private GameObject _profilePanel;
        [SerializeField] private GameObject _positionsPanel;

        public enum OnboardingStep { Profile, Positions }

        protected override void Awake()
        {
            persistent = false;
            base.Awake();
        }

        private void Start() => ShowStep(OnboardingStep.Profile);

        public void ShowStep(OnboardingStep step)
        {
            if (_profilePanel)    _profilePanel.SetActive(step   == OnboardingStep.Profile);
            if (_positionsPanel)  _positionsPanel.SetActive(step == OnboardingStep.Positions);
        }

        public void GoToPositions() => ShowStep(OnboardingStep.Positions);
    }
}
