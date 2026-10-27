using System.Collections.Generic;
using System.Threading.Tasks;
using Champions.Data.DTOs;

namespace Champions.Services
{
    /// <summary>Home content, videos, and training endpoints.</summary>
    public interface IContentService
    {
        /// <summary>GET /content</summary>
        Task<ContentBundle> GetContentAsync();

        /// <summary>GET /videos</summary>
        Task<List<Reel>> GetReelsAsync();

        /// <summary>GET /trainings</summary>
        Task<List<Training>> GetTrainingsAsync();

        /// <summary>POST /start-training</summary>
        Task StartTrainingAsync(int trainingChallengeId);

        /// <summary>POST /finish-training</summary>
        Task FinishTrainingAsync(int trainingChallengeId);

        /// <summary>GET /reports</summary>
        Task<WeeklyProgress> GetWeeklyProgressAsync();

        /// <summary>POST /new-report (re-evaluation request)</summary>
        Task RequestReEvaluationAsync();
    }
}
