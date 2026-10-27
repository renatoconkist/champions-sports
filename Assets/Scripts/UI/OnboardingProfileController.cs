using Champions.Data.DTOs;
using Champions.Services;
using Champions.Systems;
using Conkist.GDK;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Champions.UI
{
    /// <summary>
    /// Drives Onboarding step 1 — profile creation.
    /// Submits name to /update-profile then advances to the positions step.
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class OnboardingProfileController : UIDocumentBehaviour
    {
        private TextField _firstNameField, _lastNameField;
        private Button    _nextButton, _changeAvatarButton;

        protected override void OnDocumentReady()
        {
            var root = Document.rootVisualElement;
            _firstNameField     = root.Q<TextField>("first-name-field");
            _lastNameField      = root.Q<TextField>("last-name-field");
            _nextButton         = root.Q<Button>("next-button");
            _changeAvatarButton = root.Q<Button>("change-avatar-button");

            _nextButton?.RegisterCallback<ClickEvent>(_ => OnNextClicked());
            _changeAvatarButton?.RegisterCallback<ClickEvent>(_ => OnChangeAvatarClicked());
        }

        private async void OnNextClicked()
        {
            string firstName = _firstNameField?.value?.Trim() ?? "";
            string lastName  = _lastNameField?.value?.Trim()  ?? "";

            if (string.IsNullOrEmpty(firstName))
            {
                Debug.LogWarning("[OnboardingProfileController] Nome obrigatório.");
                return;
            }

            if (_nextButton != null) _nextButton.SetEnabled(false);

            try
            {
                await ProfileSystem.Instance.UpdateNameAsync(firstName, lastName, destroyCancellationToken);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[OnboardingProfileController] {ex.Message}");
            }
            finally
            {
                if (_nextButton != null) _nextButton.SetEnabled(true);
            }

            OnboardingSceneManager.Instance.GoToPositions();
        }

        private void OnChangeAvatarClicked()
        {
            // TODO: open NativeGallery / photo picker
            Debug.Log("[OnboardingProfileController] Avatar picker — not yet implemented.");
        }
    }
}
