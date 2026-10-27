using System;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace Champions.Data.ViewModels
{
    /// <summary>
    /// Dashboard / Home tab ViewModel.
    /// Exposes the daily content bundle, weekly progress and quick-access slots.
    /// </summary>
    [CreateAssetMenu(fileName = "DashboardViewModel", menuName = "Champions/ViewModels/Dashboard")]
    public class DashboardViewModel : ScriptableObject, INotifyBindablePropertyChanged
    {
        public event EventHandler<BindablePropertyChangedEventArgs> propertyChanged;

        [SerializeField] private string _greetingName;
        [SerializeField] private int _weeklyTrainingsCompleted;
        [SerializeField] private int _weeklyTrainingsGoal = 5;
        [SerializeField] private bool _isLoading;
        [SerializeField] private string _errorMessage;

        [CreateProperty] public string GreetingName
        {
            get => _greetingName;
            set { _greetingName = value; Notify(nameof(GreetingName)); }
        }

        [CreateProperty] public int WeeklyTrainingsCompleted
        {
            get => _weeklyTrainingsCompleted;
            set { _weeklyTrainingsCompleted = value; Notify(nameof(WeeklyTrainingsCompleted)); }
        }

        [CreateProperty] public int WeeklyTrainingsGoal
        {
            get => _weeklyTrainingsGoal;
            set { _weeklyTrainingsGoal = value; Notify(nameof(WeeklyTrainingsGoal)); }
        }

        [CreateProperty] public float WeeklyProgress =>
            _weeklyTrainingsGoal > 0 ? (float)_weeklyTrainingsCompleted / _weeklyTrainingsGoal : 0f;

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

        private void Notify(string p) =>
            propertyChanged?.Invoke(this, new BindablePropertyChangedEventArgs(p));
    }
}
