using System.Collections.Generic;
using Conkist.GDK.Serialization;

namespace Champions.Data.DTOs
{
    /// <summary>Player profile, account info and game progression from GET /profile.</summary>
    [System.Serializable]
    public class BackendProfile : JsonSerializableBase<BackendProfile>
    {
        public int id;
        public string given_name;
        public string last_name;
        public int age;
        public string avatar_url;
        public AvatarSettings avatar_settings;
        public List<string> positions;
        // Progression fields returned by /profile
        public int level;
        public int xp;
        public int coins;
        // Wallet/XP — GAP: old API nests under profile; a dedicated /wallet may be needed
        public float skill_tree_progress; // 0-1
    }

    [System.Serializable]
    public class AvatarSettings : JsonSerializableBase<AvatarSettings>
    {
        public string teste_cor;
        public bool show_email;
        public NotificationSettings notifications;
    }

    [System.Serializable]
    public class NotificationSettings : JsonSerializableBase<NotificationSettings>
    {
        public bool email;
        public bool sms;
    }

    /// <summary>Request body for POST /update-profile.</summary>
    [System.Serializable]
    public class UpdateProfileRequest : JsonSerializableBase<UpdateProfileRequest>
    {
        public string given_name;
        public string last_name;
        public int age;
        public AvatarSettings avatar_settings;
        public List<string> positions;
    }

    /// <summary>Request body for POST /profile/positions.</summary>
    [System.Serializable]
    public class SavePositionsRequest : JsonSerializableBase<SavePositionsRequest>
    {
        public List<string> positions;
    }
}
