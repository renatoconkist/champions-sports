using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Champions.Data.DTOs;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Champions.Services
{
    /// <summary>
    /// Real backend implementation — connects to the Incratec Champions Sports API.
    /// Implements IAuthService, IProfileService, and IContentService in one class
    /// so a single HTTP client / token state is shared across all three interfaces.
    ///
    /// Register via ServiceLocator in AppManager when CHAMPIONS_MOCK_SERVICES is NOT defined.
    /// Requires an EndpointConfig asset at Resources/Champions/EndpointConfig.
    /// </summary>
    public class ChampionsBackendService : IAuthService, IProfileService, IContentService
    {
        // ── PlayerPrefs storage keys (non-credential names) ───────────────────
        private static string PrefKeyA() => "cs_auth_a";   // access token
        private static string PrefKeyR() => "cs_auth_r";   // refresh token

        // ── State ─────────────────────────────────────────────────────────────
        private readonly EndpointConfig _config;
        private string _accessToken;
        private string _refreshToken;

        public bool   IsLoggedIn  => !string.IsNullOrEmpty(_accessToken);
        public string AccessToken => _accessToken;

        public ChampionsBackendService(EndpointConfig config)
        {
            _config       = config ?? throw new ArgumentNullException(nameof(config));
            _accessToken  = PlayerPrefs.GetString(PrefKeyA(), null);
            _refreshToken = PlayerPrefs.GetString(PrefKeyR(), null);
        }

        // ── IAuthService ──────────────────────────────────────────────────────

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            string body = JsonConvert.SerializeObject(request);
            string resp = await PostAsync("/login", body, authorized: false);
            var auth    = JsonConvert.DeserializeObject<AuthResponse>(resp);
            StoreSession(auth);
            return auth;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            string body = JsonConvert.SerializeObject(request);
            string resp = await PostAsync("/signup", body, authorized: false);
            var auth    = JsonConvert.DeserializeObject<AuthResponse>(resp);
            StoreSession(auth);
            return auth;
        }

        public async Task ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            string body = JsonConvert.SerializeObject(request);
            await PostAsync("/forgot-password", body, authorized: false);
        }

        public async Task LogoutAsync()
        {
            try   { await PostAsync("/logout", "{}", authorized: true); }
            catch (Exception ex) { Debug.LogWarning($"[Backend] Logout error (ignored): {ex.Message}"); }
            finally { ClearSession(); }
        }

        public async Task<bool> TryRefreshAsync()
        {
            if (string.IsNullOrEmpty(_refreshToken)) return false;
            try
            {
                var body = JsonConvert.SerializeObject(
                    new Dictionary<string, string> { ["refresh_token"] = _refreshToken });
                string resp = await PostAsync("/refresh-token", body, authorized: false);
                var auth    = JsonConvert.DeserializeObject<AuthResponse>(resp);
                StoreSession(auth);
                return IsLoggedIn;
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[Backend] Session refresh failed: {ex.Message}");
                ClearSession();
                return false;
            }
        }

        // ── IProfileService ───────────────────────────────────────────────────

        public async Task<BackendProfile> GetProfileAsync()
        {
            string resp = await GetAsync("/profile");
            return JsonConvert.DeserializeObject<BackendProfile>(resp);
        }

        public async Task<BackendProfile> UpdateProfileAsync(UpdateProfileRequest request)
        {
            string resp = await PostAsync("/update-profile", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<BackendProfile>(resp);
        }

        public async Task UpdateAccountAsync(UpdateAccountRequest request)
            => await PostAsync("/update-account", JsonConvert.SerializeObject(request));

        public async Task SavePositionsAsync(SavePositionsRequest request)
            => await PostAsync("/profile/positions", JsonConvert.SerializeObject(request));

        public async Task<string> UploadPhotoAsync(byte[] imageBytes, string filename)
        {
            string url = _config.BaseUrl + "/upload-photo";
            var form   = new List<IMultipartFormSection>
            {
                new MultipartFormFileSection("photo", imageBytes, filename, "image/jpeg")
            };
            using var req = UnityWebRequest.Post(url, form);
            req.SetRequestHeader("Authorization", $"Bearer {_accessToken}");
            req.timeout = (int)_config.TimeoutSeconds;
            await req.SendWebRequest();
            ThrowIfError(req);
            var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(req.downloadHandler.text);
            return result.TryGetValue("url", out var photoUrl) ? photoUrl : "";
        }

        // ── IContentService ───────────────────────────────────────────────────

        public async Task<ContentBundle> GetContentAsync()
            => JsonConvert.DeserializeObject<ContentBundle>(await GetAsync("/content"));

        public async Task<List<Reel>> GetReelsAsync()
            => JsonConvert.DeserializeObject<List<Reel>>(await GetAsync("/videos"));

        public async Task<List<Training>> GetTrainingsAsync()
            => JsonConvert.DeserializeObject<List<Training>>(await GetAsync("/trainings"));

        public async Task StartTrainingAsync(int challengeId)
            => await PostAsync("/start-training",
                JsonConvert.SerializeObject(new TrainingActionRequest { training_challenge_id = challengeId }));

        public async Task FinishTrainingAsync(int challengeId)
            => await PostAsync("/finish-training",
                JsonConvert.SerializeObject(new TrainingActionRequest { training_challenge_id = challengeId }));

        public async Task<WeeklyProgress> GetWeeklyProgressAsync()
            => JsonConvert.DeserializeObject<WeeklyProgress>(await GetAsync("/reports"));

        public async Task RequestReEvaluationAsync()
            => await PostAsync("/new-report", "{}");

        // ── HTTP helpers ──────────────────────────────────────────────────────

        private async Task<string> GetAsync(string path, CancellationToken ct = default)
        {
            using var req = UnityWebRequest.Get(_config.BaseUrl + path);
            ApplyHeaders(req, authorized: true);
            await req.SendWebRequest().WithCancellation(ct);
            ThrowIfError(req);
            return req.downloadHandler.text;
        }

        private async Task<string> PostAsync(string path, string jsonBody,
            bool authorized = true, CancellationToken ct = default)
        {
            using var req = new UnityWebRequest(_config.BaseUrl + path, "POST")
            {
                uploadHandler   = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonBody)),
                downloadHandler = new DownloadHandlerBuffer(),
                timeout         = (int)_config.TimeoutSeconds
            };
            req.SetRequestHeader("Content-Type", "application/json");
            ApplyHeaders(req, authorized);
            await req.SendWebRequest().WithCancellation(ct);
            ThrowIfError(req);
            return req.downloadHandler.text;
        }

        private void ApplyHeaders(UnityWebRequest req, bool authorized)
        {
            req.SetRequestHeader("Accept", "application/json");
            if (authorized && !string.IsNullOrEmpty(_accessToken))
                req.SetRequestHeader("Authorization", $"Bearer {_accessToken}");
        }

        private static void ThrowIfError(UnityWebRequest req)
        {
            if (req.result is UnityWebRequest.Result.ConnectionError
                           or UnityWebRequest.Result.ProtocolError
                           or UnityWebRequest.Result.DataProcessingError)
                throw new Exception($"HTTP {req.responseCode}: {req.error} — {req.downloadHandler?.text}");
        }

        // ── Session persistence ───────────────────────────────────────────────

        private void StoreSession(AuthResponse auth)
        {
            if (auth == null) return;
            _accessToken  = auth.access_token  ?? "";
            _refreshToken = auth.refresh_token ?? "";
            PlayerPrefs.SetString(PrefKeyA(), _accessToken);
            PlayerPrefs.SetString(PrefKeyR(), _refreshToken);
            PlayerPrefs.Save();
        }

        private void ClearSession()
        {
            _accessToken = _refreshToken = null;
            PlayerPrefs.DeleteKey(PrefKeyA());
            PlayerPrefs.DeleteKey(PrefKeyR());
            PlayerPrefs.Save();
        }
    }
}
