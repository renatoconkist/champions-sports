using System.Collections.Generic;
using System.Threading;
using Champions.Data.DTOs;
using Champions.Data.ViewModels;
using Champions.Services;
using Conkist.GDK;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Champions.Systems
{
    /// <summary>
    /// Fetches and caches the player profile; maps DTOs to ViewModels.
    /// </summary>
    public class ProfileSystem : PureSingleton<ProfileSystem>
    {
        private IProfileService _service;
        public ProfileViewModel ViewModel { get; private set; }

        public ProfileSystem()
        {
            _service = ServiceLocator.Get<IProfileService>();
            // ViewModel is a ScriptableObject asset — loaded on first access via Init()
            // Call Init() from a MonoBehaviour after the asset is available.
        }

        /// <summary>
        /// Call from AppManager or BootSystem after Unity's main thread is ready
        /// so Resources.Load can be used safely.
        /// </summary>
        public void Init()
        {
            // Asset lives at: Assets/Resources/Champions/ProfileViewModel.asset
            // Created automatically by ChampionsProjectSetup step 4.
            ViewModel = Resources.Load<ProfileViewModel>("Champions/ProfileViewModel");
            if (ViewModel == null)
                Debug.LogWarning("[ProfileSystem] ProfileViewModel not found at Resources/Champions/ProfileViewModel. Run Champions → Setup → 4 – Create Missing Assets.");
        }

        /// <summary>Fetch profile from backend and push to ViewModel.</summary>
        public async UniTask RefreshAsync(CancellationToken ct = default)
        {
            try
            {
                var profile = await _service.GetProfileAsync().AsUniTask().AttachExternalCancellation(ct);
                ApplyToViewModel(profile);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[ProfileSystem] Refresh failed: {ex.Message}");
            }
        }

        /// <summary>Update display name and push to backend.</summary>
        public async UniTask UpdateNameAsync(string firstName, string lastName, CancellationToken ct = default)
        {
            var req     = new Data.DTOs.UpdateProfileRequest { given_name = firstName, last_name = lastName };
            var updated = await _service.UpdateProfileAsync(req).AsUniTask().AttachExternalCancellation(ct);
            ApplyToViewModel(updated);
        }

        /// <summary>Save selected positions to backend.</summary>
        public async UniTask SavePositionsAsync(string[] positions, CancellationToken ct = default)
        {
            var req = new SavePositionsRequest { positions = new List<string>(positions) };
            await _service.SavePositionsAsync(req).AsUniTask().AttachExternalCancellation(ct);

            if (ViewModel != null)
                ViewModel.Positions = new List<string>(positions);
        }

        private void ApplyToViewModel(BackendProfile p)
        {
            if (ViewModel == null || p == null) return;
            ViewModel.DisplayName = $"{p.given_name} {p.last_name}".Trim();
            ViewModel.AvatarUrl   = p.avatar_url ?? "";
            ViewModel.Level       = p.level;
            ViewModel.XP          = p.xp;
            ViewModel.Coins       = p.coins;
            // xp_next not in DTO — approximate: level * 500 XP per level
            int xpNext = Mathf.Max(p.level * 500, 500);
            ViewModel.XPProgress  = Mathf.Clamp01((float)p.xp / xpNext);

            if (p.positions != null)
                ViewModel.Positions = new List<string>(p.positions);
        }
    }
}
