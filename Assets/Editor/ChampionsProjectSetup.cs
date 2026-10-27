using System.IO;
using Champions.Data.ViewModels;
using Champions.Services;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.U2D.Sprites;
using UnityEngine;
using UnityEngine.UIElements;

namespace Champions.Editor
{
    /// <summary>
    /// One-time project setup menu.
    /// Menu: Champions / Setup / 1..5 (run in order) or "Run All Setup".
    /// </summary>
    public static class ChampionsProjectSetup
    {
        // ─── Paths ────────────────────────────────────────────────────────────

        private const string ScenesDir        = "Assets/Scenes";
        private const string UIDir            = "Assets/UI";
        private const string ScreensDir       = UIDir + "/Screens";
        private const string AssetsDir        = "Assets/Resources/Champions";
        private const string PanelSettingsPath = UIDir + "/ChampionsPanelSettings.asset";

        // ─── Run All ──────────────────────────────────────────────────────────

        [MenuItem("Champions/Setup/▶ Run All Setup (1–5)")]
        public static void RunAll()
        {
            CreateAllScenes();
            ReimportUISprites();
            AutoSliceSpritesheets();
            CreateMissingAssets();
            SetupSceneContent();
            Debug.Log("[ChampionsSetup] ✅ Full setup complete.");
        }

        // ─── 1. Scenes ────────────────────────────────────────────────────────

        [MenuItem("Champions/Setup/1 – Create All Scenes")]
        public static void CreateAllScenes()
        {
            if (!Directory.Exists(ScenesDir))
                Directory.CreateDirectory(ScenesDir);

            string[] names = { "Boot", "Auth", "Loading", "Onboarding", "Main" };
            foreach (var name in names)
                CreateEmptyScene(ScenesDir, name);

            var buildScenes = new EditorBuildSettingsScene[names.Length];
            for (int i = 0; i < names.Length; i++)
                buildScenes[i] = new EditorBuildSettingsScene($"{ScenesDir}/{names[i]}.unity", true);
            EditorBuildSettings.scenes = buildScenes;

            AssetDatabase.Refresh();
            Debug.Log("[ChampionsSetup] Scenes created and registered in Build Settings.");
        }

        private static void CreateEmptyScene(string dir, string name)
        {
            string path = $"{dir}/{name}.unity";
            if (File.Exists(path)) { Debug.Log($"[ChampionsSetup] {name}.unity exists — skipped."); return; }
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
            EditorSceneManager.SaveScene(scene, path);
            EditorSceneManager.CloseScene(scene, true);
            Debug.Log($"[ChampionsSetup] Created {path}");
        }

        // ─── 2. Reimport sprites ──────────────────────────────────────────────

        [MenuItem("Champions/Setup/2 – Reimport UISprites")]
        public static void ReimportUISprites()
        {
            string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets/Art" });
            foreach (var guid in guids)
                AssetDatabase.ImportAsset(AssetDatabase.GUIDToAssetPath(guid), ImportAssetOptions.ForceUpdate);
            AssetDatabase.Refresh();
            Debug.Log($"[ChampionsSetup] Reimported {guids.Length} art textures.");
        }

        // ─── 3. Slice spritesheets ────────────────────────────────────────────

        [MenuItem("Champions/Setup/3 – Auto-Slice Spritesheets")]
        public static void AutoSliceSpritesheets()
        {
            var sheets = new (string name, int cellW, int cellH)[]
            {
                ("Icons",             0,   0),
                ("shields",           0,   0),
                ("SkillBadge-Samples",122, 95),
                ("RewardItemImage",   75,  78),
                ("RewardItemSlot",    87,  90),
                ("PositionToggle",   110, 114),
            };

            var factory = new SpriteDataProviderFactories();
            factory.Init();

            foreach (var (name, cellW, cellH) in sheets)
            {
                string path     = $"Assets/Art/UISprites/{name}.png";
                var    importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer == null) { Debug.LogWarning($"[ChampionsSetup] Not found: {path}"); continue; }

                importer.spriteImportMode = SpriteImportMode.Multiple;

                var dataProvider = factory.GetSpriteEditorDataProviderFromObject(importer);
                dataProvider.InitSpriteEditorDataProvider();

                var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                SpriteRect[] rects = (cellW > 0 && cellH > 0)
                    ? GenerateGridRects(tex, cellW, cellH, name)
                    : GenerateAlphaRects(tex, name);

                dataProvider.SetSpriteRects(rects);
                dataProvider.Apply();
                new SerializedObject(importer).ApplyModifiedPropertiesWithoutUndo();
                importer.SaveAndReimport();

                Debug.Log($"[ChampionsSetup] Sliced {name} → {rects.Length} sprite(s).");
            }
            AssetDatabase.Refresh();
        }

        private static SpriteRect[] GenerateGridRects(Texture2D tex, int cw, int ch, string baseName)
        {
            int cols = tex.width / cw, rows = tex.height / ch;
            var list = new System.Collections.Generic.List<SpriteRect>();
            int idx  = 0;
            for (int row = rows - 1; row >= 0; row--)
                for (int col = 0; col < cols; col++)
                    list.Add(new SpriteRect
                    {
                        name      = $"{baseName}_{idx++}",
                        rect      = new Rect(col * cw, row * ch, cw, ch),
                        pivot     = new Vector2(0.5f, 0.5f),
                        alignment = SpriteAlignment.Center,
                        spriteID  = GUID.Generate()
                    });
            return list.ToArray();
        }

        private static SpriteRect[] GenerateAlphaRects(Texture2D tex, string baseName)
        {
            int w = tex.width, h = tex.height;
            if (w * 4 < h)
            {
                int count = h / w;
                var list  = new System.Collections.Generic.List<SpriteRect>();
                for (int i = 0; i < count; i++)
                    list.Add(new SpriteRect
                    {
                        name      = $"{baseName}_{i}",
                        rect      = new Rect(0, i * w, w, w),
                        pivot     = new Vector2(0.5f, 0.5f),
                        alignment = SpriteAlignment.Center,
                        spriteID  = GUID.Generate()
                    });
                return list.ToArray();
            }
            Debug.LogWarning($"[ChampionsSetup] {baseName}: auto-detect failed — slice manually in Sprite Editor.");
            return new[] { new SpriteRect { name = baseName, rect = new Rect(0, 0, w, h), pivot = new Vector2(0.5f, 0.5f), spriteID = GUID.Generate() } };
        }

        // ─── 4. Create missing assets ─────────────────────────────────────────

        [MenuItem("Champions/Setup/4 – Create Missing Assets")]
        public static void CreateMissingAssets()
        {
            if (!Directory.Exists(UIDir)) Directory.CreateDirectory(UIDir);
            if (!Directory.Exists(AssetsDir)) Directory.CreateDirectory(AssetsDir);

            CreatePanelSettings();
            CreateViewModel<LoginViewModel>(   "LoginViewModel");
            CreateViewModel<DashboardViewModel>("DashboardViewModel");
            CreateViewModel<ProfileViewModel>( "ProfileViewModel");
            CreateScriptableObject<EndpointConfig>("EndpointConfig");

            // Ensure CHAMPIONS_MOCK_SERVICES is defined for development builds.
            AddDefineSymbol("CHAMPIONS_MOCK_SERVICES");

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[ChampionsSetup] Assets created in Assets/Resources/Champions/. Mock define added.");
        }

        private static void AddDefineSymbol(string symbol)
        {
            var targets = new[]
            {
                UnityEditor.Build.NamedBuildTarget.Android,
                UnityEditor.Build.NamedBuildTarget.iOS,
                UnityEditor.Build.NamedBuildTarget.Standalone,
                UnityEditor.Build.NamedBuildTarget.WebGL,
            };
            foreach (var target in targets)
            {
                PlayerSettings.GetScriptingDefineSymbols(target, out var defines);
                var list = new System.Collections.Generic.List<string>(defines);
                if (!list.Contains(symbol))
                {
                    list.Add(symbol);
                    PlayerSettings.SetScriptingDefineSymbols(target, list.ToArray());
                }
            }
            Debug.Log($"[ChampionsSetup] Define '{symbol}' added to all build targets.");
        }

        private static void CreatePanelSettings()
        {
            if (File.Exists(PanelSettingsPath)) return;
            var ps = ScriptableObject.CreateInstance<PanelSettings>();
            ps.scaleMode          = PanelScaleMode.ScaleWithScreenSize;
            ps.referenceResolution = new Vector2Int(1080, 1920);
            ps.match              = 0f;    // match width
            ps.referenceSpritePixelsPerUnit = 100f;
            AssetDatabase.CreateAsset(ps, PanelSettingsPath);
            Debug.Log($"[ChampionsSetup] Created {PanelSettingsPath}");
        }

        private static void CreateViewModel<T>(string fileName) where T : ScriptableObject
        {
            string path = $"{AssetsDir}/{fileName}.asset";
            if (File.Exists(path)) return;
            var asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            Debug.Log($"[ChampionsSetup] Created {path}");
        }

        private static void CreateScriptableObject<T>(string fileName) where T : ScriptableObject
        {
            string path = $"{AssetsDir}/{fileName}.asset";
            if (File.Exists(path)) return;
            var asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            Debug.Log($"[ChampionsSetup] Created {path}");
        }

        // ─── 5. Setup scene content ───────────────────────────────────────────

        [MenuItem("Champions/Setup/5 – Setup Scene Content")]
        public static void SetupSceneContent()
        {
            var panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>(PanelSettingsPath);
            if (panelSettings == null)
            {
                Debug.LogError("[ChampionsSetup] PanelSettings not found — run step 4 first.");
                return;
            }

            var loginVM     = LoadAsset<LoginViewModel>(   "Assets/Resources/Champions/LoginViewModel.asset");
            var dashVM      = LoadAsset<DashboardViewModel>("Assets/Resources/Champions/DashboardViewModel.asset");
            var profileVM   = LoadAsset<ProfileViewModel>( "Assets/Resources/Champions/ProfileViewModel.asset");

            SetupBootScene    (panelSettings);
            SetupAuthScene    (panelSettings, loginVM);
            SetupLoadingScene (panelSettings);
            SetupOnboardingScene(panelSettings, profileVM);
            SetupMainScene    (panelSettings, dashVM, profileVM);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[ChampionsSetup] Scene content setup complete.");
        }

        private static T LoadAsset<T>(string path) where T : Object
        {
            var a = AssetDatabase.LoadAssetAtPath<T>(path);
            if (a == null) Debug.LogWarning($"[ChampionsSetup] Asset not found: {path}");
            return a;
        }

        // ── Boot ──

        private static void SetupBootScene(PanelSettings ps)
        {
            string path  = $"{ScenesDir}/Boot.unity";
            var    scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);

            if (FindOrCreate<Champions.Systems.AppManager>(scene, "_AppManager") != null &&
                FindOrCreate<Champions.Systems.BootSystem> (scene, "_BootManager") != null)
                Debug.Log("[ChampionsSetup] Boot scene set up.");

            EditorSceneManager.SaveScene(scene);
            EditorSceneManager.CloseScene(scene, true);
        }

        // ── Auth ──

        private static void SetupAuthScene(PanelSettings ps, LoginViewModel loginVM)
        {
            string path  = $"{ScenesDir}/Auth.unity";
            var    scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);

            // Root manager
            var managerGO  = FindOrCreateGO(scene, "_AuthManager");
            var manager    = GetOrAdd<Champions.Systems.AuthSceneManager>(managerGO);

            // Panels (children of _AuthManager)
            var loginPanel    = CreateUIPanel(ps, managerGO, "_LoginPanel",          "LoginScreen.uxml",          true);
            var registerPanel = CreateUIPanel(ps, managerGO, "_RegisterPanel",       "RegisterScreen.uxml",       false);
            var forgotPanel   = CreateUIPanel(ps, managerGO, "_ForgotPasswordPanel", "ForgotPasswordScreen.uxml", false);
            var supportPanel  = CreateUIPanel(ps, managerGO, "_SupportPanel",        "SupportScreen.uxml",        false);

            // Wire LoginController viewmodel
            var loginCtrl = loginPanel.GetComponentInChildren<Champions.UI.LoginController>(true);
            if (loginCtrl != null && loginVM != null)
                SetField(loginCtrl, "_viewModel", loginVM);

            // Wire AuthSceneManager panel refs
            SetField(manager, "_loginPanel",          loginPanel);
            SetField(manager, "_registerPanel",       registerPanel);
            SetField(manager, "_forgotPasswordPanel", forgotPanel);
            SetField(manager, "_supportPanel",        supportPanel);

            EditorSceneManager.SaveScene(scene);
            EditorSceneManager.CloseScene(scene, true);
            Debug.Log("[ChampionsSetup] Auth scene set up.");
        }

        // ── Loading ──

        private static void SetupLoadingScene(PanelSettings ps)
        {
            string path  = $"{ScenesDir}/Loading.unity";
            var    scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);

            CreateUIPanelInScene(ps, scene, "LoadingDocument", "LoadingScreen.uxml", true);

            EditorSceneManager.SaveScene(scene);
            EditorSceneManager.CloseScene(scene, true);
            Debug.Log("[ChampionsSetup] Loading scene set up.");
        }

        // ── Onboarding ──

        private static void SetupOnboardingScene(PanelSettings ps, ProfileViewModel profileVM)
        {
            string path  = $"{ScenesDir}/Onboarding.unity";
            var    scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);

            var managerGO = FindOrCreateGO(scene, "_OnboardingManager");
            var manager   = GetOrAdd<Champions.Systems.OnboardingSceneManager>(managerGO);

            var profilePanel    = CreateUIPanel(ps, managerGO, "_ProfilePanel",   "OnboardingProfileScreen.uxml",   true);
            var positionsPanel  = CreateUIPanel(ps, managerGO, "_PositionsPanel", "OnboardingPositionsScreen.uxml", false);

            // Wire PositionsController viewmodel
            var posCtrl = positionsPanel.GetComponentInChildren<Champions.UI.PositionsController>(true);
            if (posCtrl != null && profileVM != null)
                SetField(posCtrl, "_profileViewModel", profileVM);

            SetField(manager, "_profilePanel",   profilePanel);
            SetField(manager, "_positionsPanel", positionsPanel);

            EditorSceneManager.SaveScene(scene);
            EditorSceneManager.CloseScene(scene, true);
            Debug.Log("[ChampionsSetup] Onboarding scene set up.");
        }

        // ── Main ──

        private static void SetupMainScene(PanelSettings ps, DashboardViewModel dashVM, ProfileViewModel profileVM)
        {
            string path  = $"{ScenesDir}/Main.unity";
            var    scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);

            var managerGO = FindOrCreateGO(scene, "_MainManager");
            var manager   = GetOrAdd<Champions.Systems.MainSceneManager>(managerGO);

            // Tab bar (always visible, sort on top)
            var tabBarGO  = FindOrCreateGO(scene, "_TabBar", managerGO.transform);
            var tabBarDoc = GetOrAdd<UIDocument>(tabBarGO);
            AssignUIDocument(tabBarDoc, ps, "TabBar.uxml", sortingOrder: 10);
            GetOrAdd<Champions.UI.TabBarController>(tabBarGO);

            // Tab panels
            var homeTab          = CreateUIPanel(ps, managerGO, "_HomeTab",          "DashboardScreen.uxml",   true);
            var treinoTab        = CreateUIPanel(ps, managerGO, "_TreinoTab",        "TreinoScreen.uxml",      false);
            var conhecimentoTab  = CreateUIPanel(ps, managerGO, "_ConhecimentoTab",  "ConhecimentoScreen.uxml",false);
            var rankingTab       = CreateUIPanel(ps, managerGO, "_RankingTab",       "RankingScreen.uxml",     false);
            var perfilTab        = CreateUIPanel(ps, managerGO, "_PerfilTab",        "ProfileScreen.uxml",     false);

            // Wire DashboardController viewmodel
            var dashCtrl = homeTab.GetComponentInChildren<Champions.UI.DashboardController>(true);
            if (dashCtrl != null && dashVM != null)
                SetField(dashCtrl, "_viewModel", dashVM);

            // Wire ProfileController viewmodel
            var profileCtrl = perfilTab.GetComponentInChildren<Champions.UI.ProfileController>(true);
            if (profileCtrl != null && profileVM != null)
                SetField(profileCtrl, "_viewModel", profileVM);

            // Wire MainSceneManager tab refs
            SetField(manager, "_homeTab",         homeTab);
            SetField(manager, "_treinoTab",       treinoTab);
            SetField(manager, "_conhecimentoTab", conhecimentoTab);
            SetField(manager, "_rankingTab",      rankingTab);
            SetField(manager, "_perfilTab",       perfilTab);

            EditorSceneManager.SaveScene(scene);
            EditorSceneManager.CloseScene(scene, true);
            Debug.Log("[ChampionsSetup] Main scene set up.");
        }

        // ─── Helpers ──────────────────────────────────────────────────────────

        /// <summary>Creates a child panel GO with a UIDocument + matching controller (if any).</summary>
        private static GameObject CreateUIPanel(PanelSettings ps, GameObject parent,
            string goName, string uxmlFile, bool active)
        {
            var go = FindOrCreateGO(null, goName, parent.transform);
            go.SetActive(active);

            var doc = GetOrAdd<UIDocument>(go);
            AssignUIDocument(doc, ps, uxmlFile);

            AddControllerForUxml(go, uxmlFile);
            return go;
        }

        /// <summary>Creates a root panel GO inside a scene (no parent).</summary>
        private static GameObject CreateUIPanelInScene(PanelSettings ps,
            UnityEngine.SceneManagement.Scene scene, string goName, string uxmlFile, bool active)
        {
            var go = FindOrCreateGO(scene, goName);
            go.SetActive(active);
            var doc = GetOrAdd<UIDocument>(go);
            AssignUIDocument(doc, ps, uxmlFile);
            AddControllerForUxml(go, uxmlFile);
            return go;
        }

        private static void AssignUIDocument(UIDocument doc, PanelSettings ps, string uxmlFile, int sortingOrder = 0)
        {
            doc.panelSettings  = ps;
            doc.sortingOrder   = sortingOrder;
            var uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"{ScreensDir}/{uxmlFile}");
            if (uxml != null)
                doc.visualTreeAsset = uxml;
            else
                Debug.LogWarning($"[ChampionsSetup] UXML not found: {ScreensDir}/{uxmlFile}");
            EditorUtility.SetDirty(doc);
        }

        /// <summary>Attaches the known Controller component for a given UXML screen file.</summary>
        private static void AddControllerForUxml(GameObject go, string uxmlFile)
        {
            switch (uxmlFile)
            {
                case "LoginScreen.uxml":               GetOrAdd<Champions.UI.LoginController>(go);              break;
                case "RegisterScreen.uxml":            GetOrAdd<Champions.UI.RegisterController>(go);           break;
                case "ForgotPasswordScreen.uxml":      GetOrAdd<Champions.UI.ForgotPasswordController>(go);     break;
                case "SupportScreen.uxml":             GetOrAdd<Champions.UI.SupportController>(go);            break;
                case "DashboardScreen.uxml":           GetOrAdd<Champions.UI.DashboardController>(go);          break;
                case "TreinoScreen.uxml":              GetOrAdd<Champions.UI.TreinoController>(go);             break;
                case "ConhecimentoScreen.uxml":        GetOrAdd<Champions.UI.ConhecimentoController>(go);       break;
                case "RankingScreen.uxml":             GetOrAdd<Champions.UI.RankingController>(go);            break;
                case "ProfileScreen.uxml":             GetOrAdd<Champions.UI.ProfileController>(go);            break;
                case "OnboardingProfileScreen.uxml":   GetOrAdd<Champions.UI.OnboardingProfileController>(go);   break;
                case "OnboardingPositionsScreen.uxml": GetOrAdd<Champions.UI.PositionsController>(go);            break;
            }
        }

        // ─── Scene / GO utilities ─────────────────────────────────────────────

        private static T FindOrCreate<T>(UnityEngine.SceneManagement.Scene scene, string goName)
            where T : Component
        {
            var go = FindOrCreateGO(scene, goName);
            return GetOrAdd<T>(go);
        }

        private static GameObject FindOrCreateGO(UnityEngine.SceneManagement.Scene? scene, string name,
            Transform parent = null)
        {
            // Search existing
            if (parent != null)
            {
                var existing = parent.Find(name);
                if (existing != null) return existing.gameObject;
            }
            else if (scene.HasValue)
            {
                foreach (var root in scene.Value.GetRootGameObjects())
                    if (root.name == name) return root;
            }

            var go = new GameObject(name);

            if (parent != null)
            {
                go.transform.SetParent(parent, false);
            }
            else if (scene.HasValue)
            {
                // Ensure the new root GO lands in the right scene, not whatever is currently active.
                UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(go, scene.Value);
            }

            return go;
        }

        private static T GetOrAdd<T>(GameObject go) where T : Component
        {
            var c = go.GetComponent<T>();
            return c != null ? c : go.AddComponent<T>();
        }

        private static void SetField(Object target, string fieldName, Object value)
        {
            var so = new SerializedObject(target);
            var sp = so.FindProperty(fieldName);
            if (sp == null)
            {
                Debug.LogWarning($"[ChampionsSetup] Field '{fieldName}' not found on {target.GetType().Name}");
                return;
            }
            sp.objectReferenceValue = value;
            so.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
