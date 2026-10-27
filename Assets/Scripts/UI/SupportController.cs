using UnityEngine;
using UnityEngine.UIElements;
using Conkist.GDK;

namespace Champions.UI
{
    /// <summary>
    /// Drives the Suporte screen.
    /// Tela estática: exibe contato e abre o WhatsApp ao clicar no botão.
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class SupportController : UIDocumentBehaviour
    {
        private const string WhatsAppUrl = "https://wa.me/5541999207074";

        private Button _whatsappButton, _backButton;

        protected override void OnDocumentReady()
        {
            var root = Document.rootVisualElement;

            _whatsappButton = root.Q<Button>("whatsapp-button");
            _backButton     = root.Q<Button>("back-button");

            _whatsappButton?.RegisterCallback<ClickEvent>(_ => OnWhatsAppClicked());
            _backButton?.RegisterCallback<ClickEvent>(_ => OnBackClicked());
        }

        private static void OnWhatsAppClicked()
        {
            Debug.Log($"[SupportController] Abrindo WhatsApp: {WhatsAppUrl}");
            Application.OpenURL(WhatsAppUrl);
        }

        private static void OnBackClicked() =>
            Champions.Systems.AuthSceneManager.Instance.ShowScreen(
                Champions.Systems.AuthSceneManager.AuthScreen.Login);
    }
}
