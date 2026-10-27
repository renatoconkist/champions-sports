using Champions.Services;
using Conkist.GDK;
using UnityEngine;

namespace Champions.Systems
{
    /// <summary>
    /// Bootstraps all systems and wires services on startup.
    /// Persistent singleton — survives scene changes.
    /// Attach to a GameObject named "_AppManager" in the Boot scene.
    /// </summary>
    public class AppManager : SingletonBehaviour<AppManager>
    {
        [SerializeField] private bool _useMockServices = true;

        protected override void Awake()
        {
            // persistent defaults to true in SingletonBehaviour — no change needed
            base.Awake();
            RegisterServices();
            InitSystems();
        }

        private void RegisterServices()
        {
            // Mock services are wrapped in #if so they compile only when the
            // define symbol is present. Remove the symbol and set _useMockServices
            // to false in the Inspector to switch to real backend implementations.
#if CHAMPIONS_MOCK_SERVICES
            if (_useMockServices)
            {
                ServiceLocator.Register<IAuthService>   (new Services.Mock.MockAuthService());
                ServiceLocator.Register<IProfileService>(new Services.Mock.MockProfileService());
                ServiceLocator.Register<IContentService>(new Services.Mock.MockContentService());
                Debug.Log("[AppManager] Mock services registered.");
                return;
            }
#endif
            // Wire the real backend service when CHAMPIONS_MOCK_SERVICES is not defined.
            // The EndpointConfig asset must be filled in at Assets/Resources/Champions/EndpointConfig.
            var cfg = UnityEngine.Resources.Load<EndpointConfig>("Champions/EndpointConfig");
            if (cfg == null)
            {
                Debug.LogError("[AppManager] EndpointConfig not found at Resources/Champions/EndpointConfig. " +
                               "Run Champions → Setup → 4 – Create Missing Assets.");
                return;
            }
            var backend = new ChampionsBackendService(cfg);
            ServiceLocator.Register<IAuthService>   (backend);
            ServiceLocator.Register<IProfileService>(backend);
            ServiceLocator.Register<IContentService>(backend);
            Debug.Log("[AppManager] Real backend services registered.");
        }

        private void InitSystems()
        {
            // Trigger lazy PureSingleton construction now that services are registered.
            _ = AuthSystem.Instance;
            ProfileSystem.Instance.Init();
            Debug.Log("[AppManager] Systems initialised.");
        }
    }
}
