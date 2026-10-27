using System.Collections.Generic;
using System.Threading.Tasks;
using Champions.Data.DTOs;

namespace Champions.Services.Mock
{
    /// <summary>
    /// Fake content service. Returns placeholder cards so the UI can be built
    /// and exercised without a backend.
    /// Compiled only when CHAMPIONS_MOCK_SERVICES is defined.
    /// </summary>
#if CHAMPIONS_MOCK_SERVICES
    public class MockContentService : IContentService, IMockService
    {
        public async Task<ContentBundle> GetContentAsync()
        {
            await Task.Delay(500);
            var card1 = new ContentCard { id = 1, title = "Como fazer um drible perfeito", category = "know",  style = "blue"  };
            var card2 = new ContentCard { id = 2, title = "Força e resistência no treino",  category = "train", style = "green" };
            var card3 = new ContentCard { id = 3, title = "Mentalidade de campeão",         category = "know",  style = "gold"  };
            return new ContentBundle
            {
                featured   = new List<ContentCard> { card1 },
                news       = new List<ContentCard> { card2, card3 },
                events     = new List<ContentCard>(),
                challenges = new List<ContentCard>()
            };
        }

        public async Task<List<Reel>> GetReelsAsync()
        {
            await Task.Delay(400);
            return new List<Reel>
            {
                new() { id = 1, title = "Dribbling 101", path = "", thumbnail_url = "" },
                new() { id = 2, title = "Chute preciso",  path = "", thumbnail_url = "" }
            };
        }

        public async Task<List<Training>> GetTrainingsAsync()
        {
            await Task.Delay(400);
            return new List<Training>
            {
                new() { id = 1, title = "Treino de agilidade",    level = "iniciante",     category = "fisico" },
                new() { id = 2, title = "Condicionamento físico", level = "intermediário", category = "fisico" }
            };
        }

        public async Task StartTrainingAsync(int trainingChallengeId)  => await Task.Delay(200);
        public async Task FinishTrainingAsync(int trainingChallengeId) => await Task.Delay(200);

        public async Task<WeeklyProgress> GetWeeklyProgressAsync()
        {
            await Task.Delay(300);
            return new WeeklyProgress
            {
                week  = 1,
                score = 0.6f,
                evaluations = new List<EvaluationItem>
                {
                    new() { skill = "tecnica", value = 0.7f },
                    new() { skill = "fisico",  value = 0.5f },
                    new() { skill = "tatico",  value = 0.6f }
                }
            };
        }

        public async Task RequestReEvaluationAsync() => await Task.Delay(300);
    }
#endif
}
