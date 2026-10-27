using Champions.Data.ViewModels;
using Champions.Systems;
using Conkist.GDK;
using UnityEngine;
using UnityEngine.UIElements;

namespace Champions.UI
{
    /// <summary>
    /// Drives the Profile UXML screen inside the Main scene.
    /// dataSource = ProfileViewModel (set in Inspector).
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class ProfileController : UIDocumentBehaviour
    {
        [SerializeField] private ProfileViewModel _viewModel;

        private Button _editButton;
        private Button _logoutButton;

        protected override void OnDocumentReady()
        {
            var root = Document.rootVisualElement;

            if (_viewModel != null)
                root.dataSource = _viewModel;

            _editButton   = root.Q<Button>("edit-profile-button");
            _logoutButton = root.Q<Button>("logout-button");

            _editButton?.RegisterCallback<ClickEvent>(_ => OnEditClicked());
            _logoutButton?.RegisterCallback<ClickEvent>(_ => OnLogoutClicked());

            // Refresh on show
            _ = ProfileSystem.Instance.RefreshAsync(destroyCancellationToken);
        }

        private void OnEditClicked()
        {
            // TODO: open edit-profile overlay / sheet
            Debug.Log("[ProfileController] Edit profile — not yet implemented.");
        }

        private async void OnLogoutClicked()
        {
            await AuthSystem.Instance.LogoutAsync(destroyCancellationToken);
            NavigationSystem.GoToAuth();
        }
    }
}
