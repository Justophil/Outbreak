using System;
using System.Collections.Generic;

namespace InfimaGames.LowPolyShooterPack
{
    public class ServiceLocator
    {
        private readonly Dictionary<string, IGameService> services = new Dictionary<string, IGameService>();

        public static ServiceLocator Current { get; private set; }

        public static void Initialize()
        {
            if (Current == null)
            {
                Current = new ServiceLocator();
            }
            else
            {
                // Handle re-initialization if needed
                // Log.warn("ServiceLocator is being re-initialized.");
            }
        }

        public T Get<T>() where T : IGameService
        {
            string key = typeof(T).Name;

            if (!services.TryGetValue(key, out IGameService service))
            {
                // Log.error($"{key} not registered with {GetType().Name}");
                throw new InvalidOperationException($"{key} not registered with {GetType().Name}");
            }

            return (T)service;
        }

        public void Register<T>(T service) where T : IGameService
        {
            string key = typeof(T).Name;

            if (services.ContainsKey(key))
            {
                // Log.warn($"Service of type {key} is already registered with {GetType().Name}.");
                return;
            }

            // Add.
            services.Add(key, service);
        }

        public void Unregister<T>() where T : IGameService
        {
            string key = typeof(T).Name;

            if (!services.ContainsKey(key))
            {
                // Log.warn($"Service of type {key} is not registered with {GetType().Name}.");
                return;
            }

            services.Remove(key);
        }
    }
}