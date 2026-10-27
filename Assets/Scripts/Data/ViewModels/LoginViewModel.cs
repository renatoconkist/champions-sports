using System;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace Champions.Data.ViewModels
{
    /// <summary>Login screen ViewModel — binds to Login.uxml.</summary>
    [CreateAssetMenu(fileName = "LoginViewModel", menuName = "Champions/ViewModels/Login")]
    public class LoginViewModel : ScriptableObject, INotifyBindablePropertyChanged
    {
        public event EventHandler<BindablePropertyChangedEventArgs> propertyChanged;

        [SerializeField] private string _email;
        [SerializeField] private string _password;
        [SerializeField] private bool _isLoading;
        [SerializeField] private string _errorMessage;
        [SerializeField] private string _selectedLocale = "pt-BR";

        [CreateProperty] public string Email
        {
            get => _email;
            set { _email = value; Notify(nameof(Email)); }
        }

        [CreateProperty] public string Password
        {
            get => _password;
            set { _password = value; Notify(nameof(Password)); }
        }

        [CreateProperty] public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; Notify(nameof(IsLoading)); }
        }

        [CreateProperty] public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; Notify(nameof(ErrorMessage)); }
        }

        [CreateProperty] public string SelectedLocale
        {
            get => _selectedLocale;
            set { _selectedLocale = value; Notify(nameof(SelectedLocale)); }
        }

        private void Notify(string p) =>
            propertyChanged?.Invoke(this, new BindablePropertyChangedEventArgs(p));
    }
}
