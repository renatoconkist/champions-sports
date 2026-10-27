using System;
using System.Collections.Generic;
using UnityEngine;

namespace Champions.Services
{
    /// <summary>
    /// Lightweight static service locator.
    /// Register implementations before the first Get call (e.g. in AppManager.Awake).
    /// </summary>
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> _services = new();

        /// <summary>Register an implementation for interface T.</summary>
        public static void Register<T>(T implementation) where T : class
        {
            _services[typeof(T)] = implementation
                ?? throw new ArgumentNullException(nameof(implementation));
        }

        /// <summary>Retrieve the registered implementation for T.</summary>
        public static T Get<T>() where T : class
        {
            if (_services.TryGetValue(typeof(T), out var service))
                return (T)service;

            Debug.LogError($"[ServiceLocator] No service registered for {typeof(T).Name}.");
            return null;
        }

        /// <summary>Remove all registrations (useful for tests / scene teardown).</summary>
        public static void Clear() => _services.Clear();
    }
}
