using UnityEngine;

namespace Champions.Services
{
    /// <summary>
    /// ScriptableObject that stores the backend connection settings.
    /// Create via Assets → Create → Champions → EndpointConfig.
    /// Set the real URL in this asset before switching off mock services.
    /// </summary>
    [CreateAssetMenu(fileName = "EndpointConfig", menuName = "Champions/EndpointConfig")]
    public class EndpointConfig : ScriptableObject
    {
        [Tooltip("Base URL of the Champions Sports API. Example: https://api.incratec.com.br/v1")]
        [SerializeField] private string _baseUrl = "https://PLACEHOLDER.example.com/api/v1";

        [Tooltip("HTTP request timeout in seconds.")]
        [SerializeField] private float _timeoutSeconds = 30f;

        public string BaseUrl         => _baseUrl;
        public float  TimeoutSeconds  => _timeoutSeconds;
    }
}
