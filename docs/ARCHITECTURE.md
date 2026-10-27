# Champions Sports — Architecture Guide

For new developers joining the project.

---

## Naming conventions (Renato's rules — apply everywhere)

| Kind | Definition | Base class | Example |
|------|-----------|------------|---------|
| **System** | Scene-independent; callable by any script; may control something temporary in a scene | `PureSingleton` or static C# | `AuthSystem`, `NavigationSystem` |
| **Manager** | Scene-bound; can force itself to persist; controls one aspect of the whole app | `SingletonBehaviour<T>` (`persistent: true`) | `AppManager`, `AuthSceneManager` |
| **Controller** | Lives inside an object or group; manages that object's sub-elements | `MonoBehaviour` | `LoginController`, `TabBarController` |
| **Behaviour** | MonoBehaviour base of singletons/managers/controllers; also single-purpose tasks | `MonoBehaviour` | `SpinnerBehaviour`, `SafeAreaBehaviour` |

---

## Assembly structure

```
Champions.Data      DTOs (IJsonSerializable), ViewModels (ScriptableObject), SOs
Champions.Services  Service interfaces + mock/real implementations
Champions.Systems   Pure C# systems (no scene dependency)
Champions.UI        Controllers, Behaviours, UXML elements
```

Each assembly references only the ones below it. `Conkist.GDK` is referenced by all four.

---

## Data flow

```
Backend / Mock JSON
        ↓
  Service layer (IAuthService, IProfileService, …)
        ↓  (DTO returned)
  System layer (AuthSystem, ProfileSystem, …)
        ↓  (maps DTO → ViewModel via Mapper)
  ViewModel (ScriptableObject, INotifyBindablePropertyChanged)
        ↓  (UI Toolkit runtime data binding: dataSource = VM)
  UXML / Controller
```

Controllers read ViewModels and call **Commands** (`CommandSystem` → `CommandExecutor`).  
Commands call Services. Services never call the UI layer.

---

## Scenes and game states

| State | Scene | Entry | Exit |
|-------|-------|-------|------|
| `Boot` | Boot | App launch | Services ready → Auth |
| `Auth` | Auth | From Boot | Logged in → Loading |
| `Loading` | Loading | Between scenes | Target loaded → Onboarding or Main |
| `Onboarding` | Onboarding | First login (no saved profile) | Profile + positions saved → Main |
| `Main` | Main | Returning user or after Onboarding | — |

`GameStateManager` (GDK) drives transitions via `NavigationSystem.GoTo(GameState)`.

---

## How to add a new screen

1. Create `Assets/UI/Screens/MyScreen.uxml` and `MyScreen.uss`.
2. Add a `[UxmlElement]` root or use a standard `VisualElement`.
3. Create `Assets/Scripts/UI/MyScreenController.cs` (extends `UIDocumentBehaviour`).
4. Bind it to a ViewModel ScriptableObject (`dataSource = myViewModel`).
5. Register the screen in the owning scene's Manager (`AuthSceneManager`, `MainSceneManager`, …).
6. Add localization keys for every text string.
7. No service calls from the controller — use a Command instead.

---

## How to add a UI component

1. Create `Assets/UI/Components/MyComponent.uxml` + `MyComponent.uss`.
2. Optionally create a `[UxmlElement]` C# class in `Champions.UI` for custom attributes.
3. Add USS modifier classes for variants (color, size) — see `theme.uss` for tokens.
4. Import it in screens with `<ui:Template>` or instantiate via code.

---

## Switching mock → real services

1. In **Project Settings → Player → Scripting Define Symbols**, remove `CHAMPIONS_MOCK_SERVICES`.
2. Update `EndpointConfig.asset` with the production base URL.
3. In `AppManager.Awake()`, swap the mock registration for `ChampionsBackendService`.

All service interfaces (`IAuthService`, `IProfileService`, etc.) remain unchanged.

---

## Localization

Unity Localization package. Tables in `Assets/Localization/`.  
- Default locale: `pt-BR`
- Keys follow: `SCREEN_ELEMENT` (e.g. `LOGIN_EMAIL_PLACEHOLDER`, `HOME_WELCOME_TITLE`)
- `LocalizationSystem.SetLocale(LocaleCode)` saves choice to PlayerPrefs and triggers `GameEvent.LanguageChanged`
- Flag buttons on Login screen trigger `ChangeLanguageCommand`

---

## Secure storage

Auto-login uses the backend **refresh token**, not the user's plaintext credentials.
The token is stored through `ISecureStorage` (`Assets/Scripts/Services/ISecureStorage.cs`).

| Build type | Implementation | Notes |
|---|---|---|
| `CHAMPIONS_MOCK_SERVICES` | `PlayerPrefsSecureStorage` | **Unencrypted — dev only, never ship** |
| Production (iOS) | `NativeSecureStorage` (Keychain) | ⚠️ **Not yet implemented — handoff item** |
| Production (Android) | `NativeSecureStorage` (Keystore / EncryptedSharedPreferences) | ⚠️ **Not yet implemented — handoff item** |

Register the implementation in `AppManager.Awake()` via `ServiceLocator.Register<ISecureStorage>(...)`.
`IAuthService` implementations must use `ISecureStorage` to persist and retrieve the refresh token.
**Never store user passwords.**

---

## Placeholders and missing assets

These require delivery from the client or Renato before release:

| Asset | Used in | Notes |
|-------|---------|-------|
| App logo | Boot splash, Login | Placeholder: white "CS" text on dark background |
| Player avatars | Profile, TopBar | Placeholder: `boy.png` UISprite |
| Shirt icons per position | Position picker | Placeholder: solid colour circles; `custom-shirt.png` UISprite |
| Field spot map | Position picker | Placeholder: plain green rectangle |
| Cart icon | Store | Placeholder: `cat-strategy.png` UISprite |
| Trophy icons | Ranking | Placeholder: `BadgeBack.png` UISprite |
| Social feed content | Conhecimento | Placeholder: lorem ipsum cards |
| `EndpointConfig.asset` values | All services | Base URL, timeout — must be set before real backend testing |

---

## GDK packages used

| Package | Version | Used for |
|---------|---------|---------|
| `me.conkist.gdk.core` | 0.1.0 | SingletonBehaviour, EventManager, GameStateManager, LoadingManager, SceneManager, OverlayManager, AudioManager, CommandExecutor |
| `me.conkist.gdk.services` | 0.1.0 | BaseBackendService, CustomBackendService, EndpointConfiguration, IAuthenticate |

GDK Core requires `CONKIST_UNITASK` and `CONKIST_VCONTAINER` define symbols — both set automatically when `com.cysharp.unitask` and `jp.hadashikick.vcontainer` are present in the manifest.
