using System.Collections.Generic;
using Champions.Data.ViewModels;
using Champions.Systems;
using Conkist.GDK;
using UnityEngine;
using UnityEngine.UIElements;

namespace Champions.UI
{
    /// <summary>
    /// Drives the Position picker screen (Onboarding step 2).
    /// Lets the player pick 1–3 positions from the football-field map.
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class PositionsController : UIDocumentBehaviour
    {
        [SerializeField] private ProfileViewModel _profileViewModel;

        private readonly List<string> _selected = new();
        private Button _confirmButton;

        protected override void OnDocumentReady()
        {
            var root = Document.rootVisualElement;
            _confirmButton = root.Q<Button>("confirm-button");
            _confirmButton?.RegisterCallback<ClickEvent>(_ => OnConfirmClicked());

            // TODO: populate position buttons from a PositionConfig SO and wire toggle callbacks
        }

        private async void OnConfirmClicked()
        {
            if (_selected.Count == 0)
            {
                Debug.LogWarning("[PositionsController] No position selected.");
                return;
            }

            await ProfileSystem.Instance.SavePositionsAsync(_selected.ToArray(), destroyCancellationToken);
            NavigationSystem.GoToMain();
        }
    }
}
