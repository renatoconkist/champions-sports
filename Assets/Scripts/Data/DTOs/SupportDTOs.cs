using Conkist.GDK.Serialization;

namespace Champions.Data.DTOs
{
    /// <summary>
    /// GAP: Support. The old API has no /support endpoint.
    /// New endpoint required: POST /support (contact form submission).
    /// </summary>
    [System.Serializable]
    public class SupportRequest : JsonSerializableBase<SupportRequest>
    {
        public string subject;
        public string message;
        public string email; // pre-filled from profile
        // NEW endpoint required: POST /support
    }
}
