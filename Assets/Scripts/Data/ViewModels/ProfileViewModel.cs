using System;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace Champions.Data.ViewModels
{
    /// <summary>
    /// Player profile ViewModel for UI Toolkit runtime data binding.
    /// Assign as UIDocument.dataSource on the Profile and TopBar screens.
    /// </summary>
    [CreateAssetMenu(fileName = "ProfileViewModel", menuName = "Champions/ViewModels/Profile")]
    public class ProfileViewModel : ScriptableObject, INotifyBindablePropertyChanged
    {
        public event EventHandler<BindablePropertyChangedEventArgs> propertyChanged;

        [SerializeField] private string _displayName;
        [SerializeField] private string _avatarUrl;
        [SerializeField] private int _level;
        [SerializeField] private int _xp;
        [SerializeField] private int _coins;
        [SerializeField] private List<string> _positions = new();
        [SerializeField] private float _xpProgress; // 0–1 for the progress bar

        [CreateProperty]
        public string DisplayName
        {
            get => _displayName;
            set { _displayName = value; Notify(nameof(DisplayName)); }
        }

        [CreateProperty]
        public string AvatarUrl
        {
            get => _avatarUrl;
            set { _avatarUrl = value; Notify(nameof(AvatarUrl)); }
        }

        [CreateProperty]
        public int Level
        {
            get => _level;
            set { _level = value; Notify(nameof(Level)); }
        }

        [CreateProperty]
        public int XP
        {
            get => _xp;
            set { _xp = value; Notify(nameof(XP)); }
        }

        [CreateProperty]
        public int Coins
        {
            get => _coins;
            set { _coins = value; Notify(nameof(Coins)); }
        }

        [CreateProperty]
        public float XPProgress
        {
            get => _xpProgress;
            set { _xpProgress = Mathf.Clamp01(value); Notify(nameof(XPProgress)); }
        }

        [CreateProperty]
        public List<string> Positions
        {
            get => _positions;
            set { _positions = value; Notify(nameof(Positions)); }
        }

        private void Notify(string property) =>
            propertyChanged?.Invoke(this, new BindablePropertyChangedEventArgs(property));
    }
}
