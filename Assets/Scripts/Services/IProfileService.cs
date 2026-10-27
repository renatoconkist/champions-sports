using System.Threading.Tasks;
using Champions.Data.DTOs;

namespace Champions.Services
{
    /// <summary>Wraps profile and positions endpoints.</summary>
    public interface IProfileService
    {
        /// <summary>GET /profile</summary>
        Task<BackendProfile> GetProfileAsync();

        /// <summary>POST /update-profile</summary>
        Task<BackendProfile> UpdateProfileAsync(UpdateProfileRequest request);

        /// <summary>PATCH /update-account</summary>
        Task UpdateAccountAsync(UpdateAccountRequest request);

        /// <summary>POST /profile/positions (up to 3)</summary>
        Task SavePositionsAsync(SavePositionsRequest request);

        /// <summary>POST /upload-photo — multipart upload</summary>
        Task<string> UploadPhotoAsync(byte[] imageBytes, string filename);
    }
}
