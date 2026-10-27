using Champions.Data.ViewModels;
using Champions.Services;
using Conkist.GDK;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Champions.UI
{
    /// <summary>Drives the Dashboard / Home tab screen.</summary>
    [RequireComponent(typeof(UIDocument))]
    public class DashboardController : UIDocumentBehaviour
    {
        [SerializeField] private DashboardViewModel _viewModel;

        protected override void OnDocumentReady()
        {
            if (_viewModel != null)
                Document.rootVisualElement.dataSource = _viewModel;

            _ = LoadAsync();
        }

        private async UniTaskVoid LoadAsync()
        {
            if (_viewModel == null) return;
            _viewModel.IsLoading = true;

            try
            {
                var profileSvc  = ServiceLocator.Get<IProfileService>();
                var contentSvc  = ServiceLocator.Get<IContentService>();

                var profile     = await profileSvc.GetProfileAsync().AsUniTask()
                                                  .AttachExternalCancellation(destroyCancellationToken);
                var progress    = await contentSvc.GetWeeklyProgressAsync().AsUniTask()
                                                  .AttachExternalCancellation(destroyCancellationToken);

                _viewModel.GreetingName = profile?.given_name ?? "";

                // WeeklyProgress.score is 0–1; goal = 5 trainings/week
                if (progress != null)
                {
                    _viewModel.WeeklyTrainingsGoal      = 5;
                    _viewModel.WeeklyTrainingsCompleted = Mathf.RoundToInt(progress.score * 5f);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[DashboardController] Load failed: {ex.Message}");
                if (_viewModel != null) _viewModel.ErrorMessage = "Erro ao carregar.";
            }
            finally
            {
                if (_viewModel != null) _viewModel.IsLoading = false;
            }
        }
    }
}
