using System.Collections.Generic;
using System.Threading.Tasks;
using Champions.Data.DTOs;

namespace Champions.Services.Mock
{
    /// <summary>
    /// Fake profile service that returns a static demo profile.
    /// Compiled only when CHAMPIONS_MOCK_SERVICES is defined.
    /// </summary>
#if CHAMPIONS_MOCK_SERVICES
    public class MockProfileService : IProfileService, IMockService
    {
        private BackendProfile _profile = new()
        {
            id         = 1,
            given_name = "João",
            last_name  = "Silva",
            age        = 16,
            level      = 5,
            xp         = 1240,
            coins      = 380,
            avatar_url = "",
            positions  = new List<string> { "atacante", "meia" }
        };

        public async Task<BackendProfile> GetProfileAsync()
        {
            await Task.Delay(400);
            return _profile;
        }

        public async Task<BackendProfile> UpdateProfileAsync(UpdateProfileRequest request)
        {
            await Task.Delay(500);
            if (!string.IsNullOrEmpty(request.given_name)) _profile.given_name = request.given_name;
            if (!string.IsNullOrEmpty(request.last_name))  _profile.last_name  = request.last_name;
            if (request.age > 0)                           _profile.age        = request.age;
            return _profile;
        }

        public async Task UpdateAccountAsync(UpdateAccountRequest request)
        {
            await Task.Delay(500);
            // No email on BackendProfile — nothing to update in mock
        }

        public async Task SavePositionsAsync(SavePositionsRequest request)
        {
            await Task.Delay(400);
            _profile.positions = request.positions;
        }

        public async Task<string> UploadPhotoAsync(byte[] imageBytes, string filename)
        {
            await Task.Delay(1000);
            return "https://placeholder.example.com/avatar.jpg";
        }
    }
#endif
}
