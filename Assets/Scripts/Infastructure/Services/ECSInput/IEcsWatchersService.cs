using System.Collections.Generic;
using UnityEngine;

namespace Infastructure.Services.ECSInput
{
    public interface IEcsWatchersService
    {
        List<IEcsWatcher> EcsWatchers { get; }
        List<IEcsWatcherWindow> EcsWatchersWindows { get; }
        void RegisterWatchers(GameObject gameObject);
        void Release(IEcsWatcherWindow ecsWatcherWindow);
        bool CanOpenWindow();
    }
}