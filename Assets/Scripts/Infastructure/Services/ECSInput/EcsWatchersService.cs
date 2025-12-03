using System.Collections.Generic;
using UnityEngine;

namespace Infastructure.Services.ECSInput
{
    public class EcsWatchersService : IEcsWatchersService
    {
        public List<IEcsWatcher> EcsWatchers { get; } = new List<IEcsWatcher>();
        public List<IEcsWatcherWindow> EcsWatchersWindows { get; } = new List<IEcsWatcherWindow>();

        public void RegisterWatchers(GameObject gameObject)
        {
            foreach (IEcsWatcherWindow ecsWatcherWindow in gameObject.GetComponentsInChildren<IEcsWatcherWindow>())
            {
                if (ecsWatcherWindow is IEcsWatcher ecsWatcher && !EcsWatchers.Contains(ecsWatcher))
                    EcsWatchers.Add(ecsWatcher);
                else
                {
                    if (!EcsWatchersWindows.Contains(ecsWatcherWindow))
                        EcsWatchersWindows.Add(ecsWatcherWindow);
                }
            }
        }

        public void Release(IEcsWatcherWindow escWatcherWindow)
        {
            if (escWatcherWindow is IEcsWatcher ecsWatcher && EcsWatchers.Contains(ecsWatcher))
                EcsWatchers.Remove(ecsWatcher);
            else
            {
                if (EcsWatchersWindows.Contains(escWatcherWindow))
                    EcsWatchersWindows.Remove(escWatcherWindow);
            }
        }

        public bool CanOpenWindow()
        {
            bool canUse = true;

            foreach (IEcsWatcher ecsWatcher in EcsWatchers)
            {
                if (!ecsWatcher.CanUseEcsMenu())
                {
                    canUse = false;
                    break;
                }
            }

            return EcsWatchersWindows.Count == 0 && canUse;
        }
    }
}