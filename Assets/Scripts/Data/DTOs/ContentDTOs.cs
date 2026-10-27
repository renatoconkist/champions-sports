using System.Collections.Generic;
using Conkist.GDK.Serialization;

namespace Champions.Data.DTOs
{
    /// <summary>Home content bundle from GET /content (news, events, challenges, game data).</summary>
    [System.Serializable]
    public class ContentBundle : JsonSerializableBase<ContentBundle>
    {
        public List<ContentCard> featured;
        public List<ContentCard> news;
        public List<ContentCard> events;
        public List<ContentCard> challenges;
    }

    [System.Serializable]
    public class ContentCard : JsonSerializableBase<ContentCard>
    {
        public int id;
        public string title;
        public string subtitle;
        public string image_url;
        public string category; // "know", "train", "mission", "rank"
        public string style;    // "green", "blue", "gold", "red"
        public string footer_label;
    }

    /// <summary>Reel video item from GET /videos.</summary>
    [System.Serializable]
    public class Reel : JsonSerializableBase<Reel>
    {
        public int id;
        public string path;       // video path or URL
        public string thumbnail_url;
        public string title;
    }

    /// <summary>Training item from GET /trainings.</summary>
    [System.Serializable]
    public class Training : JsonSerializableBase<Training>
    {
        public int id;
        public string title;
        public string description;
        public string level;
        public string category;
    }

    /// <summary>Start/finish training — POST /start-training, /finish-training.</summary>
    [System.Serializable]
    public class TrainingActionRequest : JsonSerializableBase<TrainingActionRequest>
    {
        public int training_challenge_id;
    }

    /// <summary>Weekly progress / evaluation report from GET /reports.</summary>
    [System.Serializable]
    public class WeeklyProgress : JsonSerializableBase<WeeklyProgress>
    {
        public int week;
        public float score;
        public List<EvaluationItem> evaluations;
    }

    [System.Serializable]
    public class EvaluationItem : JsonSerializableBase<EvaluationItem>
    {
        public string skill;
        public float value;
    }

    /// <summary>
    /// GAP: Missions. The old API has no /missions endpoint.
    /// A new endpoint POST /missions and GET /missions is needed from the backend team.
    /// </summary>
    [System.Serializable]
    public class Mission : JsonSerializableBase<Mission>
    {
        public int id;
        public string title;
        public string description;
        public bool completed;
        public RewardItem reward;
        // NEW endpoint required: GET /missions, POST /missions/{id}/complete
    }

    /// <summary>
    /// GAP: Ranking / League. The old API has no /ranking or /league endpoint.
    /// New endpoints required: GET /ranking, GET /league.
    /// </summary>
    [System.Serializable]
    public class RankingEntry : JsonSerializableBase<RankingEntry>
    {
        public int position;
        public int account_id;
        public string username;
        public string avatar_url;
        public int score;
        public string league; // "bronze", "silver", "gold", "diamond"
    }

    /// <summary>
    /// GAP: Wallet / Coins / XP. Old API nests xp and coins under /profile.
    /// A dedicated GET /wallet endpoint is recommended for real-time balance.
    /// </summary>
    [System.Serializable]
    public class Wallet : JsonSerializableBase<Wallet>
    {
        public int coins;
        public int xp;
        public int level;
        // NEW endpoint recommended: GET /wallet
    }

    [System.Serializable]
    public class RewardItem : JsonSerializableBase<RewardItem>
    {
        public string type;  // "coins", "xp", "item"
        public int amount;
        public string item_id;
    }
}
