# Champions Sports

Mobile app for **Incratec** (client: Seu Roberto).  
Unity 6000.3.14f1 · URP · UI Toolkit · Android + iOS  
Bundle ID: `com.incratec.championssports`

---

## Requirements

| Tool | Version |
|------|---------|
| Unity Editor | 6000.3.14f1 (arm64 on macOS) |
| Unity Hub | 3.x+ |
| Xcode | 15+ (iOS builds) |
| Android Build Support | included in Editor modules |
| Unity CLI | 1.0.0-beta.12+ (`unity --version`) |

---

## Opening the project

```bash
# Via Unity CLI
unity open ~/Work/Incratec/ChampionsSports

# Via Unity Hub
# File → Open Project → select ~/Work/Incratec/ChampionsSports
```

On first open Unity will resolve all packages (may take a few minutes).  
Required packages are fetched automatically: Conkist GDK Core + Services (private GitHub),  
UniTask, VContainer, Addressables, Localization, URP 17.

---

## Running in Editor

1. Open the project in Unity 6000.3.14f1.
2. Open `Assets/Scenes/Boot.unity`.
3. Press **Play**. The app starts from the Boot splash and flows through Auth → Loading → Onboarding → Main.

> **Mock services are active by default.** Real backend requires removing the `CHAMPIONS_MOCK_SERVICES` scripting define symbol and providing a valid API base URL in `Assets/Scripts/Services/EndpointConfig.asset`.

---

## Building

### Android (dev build)

```bash
unity build ~/Work/Incratec/ChampionsSports \
  --editor-version 6000.3.14f1 \
  --target Android \
  --execute-method Champions.Editor.Builder.BuildAndroid
```

Or via Unity: **File → Build Profiles → Android → Build**.

### iOS (Xcode project)

```bash
unity build ~/Work/Incratec/ChampionsSports \
  --editor-version 6000.3.14f1 \
  --target iOS \
  --execute-method Champions.Editor.Builder.BuildIOS
```

Then open the generated Xcode project in `Build/iOS/`, add signing, and run on device.  
Signing and device install are **Renato's step** — not automated here.

---

## Running tests

```bash
unity test ~/Work/Incratec/ChampionsSports \
  --editor-version 6000.3.14f1 \
  --mode EditMode
```

---

## Project layout

```
Assets/
  Art/
    UISprites/       40 PNG sprites from the design reference
    Flags/           flag_us/br/es.png (256px) + svg-sources/
  Fonts/
    Inter/           Regular, Medium, SemiBold, Bold, Variable TTF + SDF assets
    Antonio/         Variable TTF + SDF asset
  Scenes/            Boot, Auth, Loading, Onboarding, Main
  Scripts/
    Data/            DTOs, ViewModels (ScriptableObjects), ScriptableObject definitions
    Services/        Service interfaces + mock implementations
    Systems/         Pure C# systems (Auth, Profile, Navigation, Localization, …)
    UI/              Controllers, Behaviours, custom UxmlElement components
  UI/
    Theme/           PanelSettings, theme.uss (design tokens)
    Components/      Reusable UXML + USS (Button, Card, TabBar, …)
    Screens/         Per-screen UXML + USS
Packages/
  manifest.json      All UPM dependencies (GDK via git URL, others via registry)
docs/
  api/
    ENDPOINTS.md     API mapping and gaps for backend team
    openapi.yaml     OpenAPI 3.0 draft (TBD)
  ARCHITECTURE.md    Code architecture guide for new developers
THIRD_PARTY.md       Licences for flags and fonts
```

---

## Switching to the real backend

1. Remove `CHAMPIONS_MOCK_SERVICES` from **Project Settings → Player → Scripting Define Symbols**.
2. Create or update `Assets/Scripts/Services/EndpointConfig.asset` with the production base URL.
3. Ensure `ChampionsBackendService` is registered instead of `MockAuthService` in `AppManager.cs`.

---

## Localization

Three locales: **pt-BR** (default), **en-US**, **es-ES**.  
String tables: `Assets/Localization/` (created by Unity Localization package).  
Language selector is on the Login screen (flag buttons).

---

## Open questions / placeholders

See `docs/ARCHITECTURE.md#placeholders` for the full list.

### Handoff items (blocking release)

| Item | Owner | Notes |
|---|---|---|
| `NativeSecureStorage` (iOS Keychain) | Renato / native plugin | Replace `PlayerPrefsSecureStorage` before iOS release |
| `NativeSecureStorage` (Android Keystore) | Renato / native plugin | Same for Android release |
| Theme colours in `theme.uss` | Renato (awaiting client PNGs) | Tokens present; values are placeholders |
