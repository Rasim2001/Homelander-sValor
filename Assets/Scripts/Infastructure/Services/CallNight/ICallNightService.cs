using System;

namespace Infastructure.Services.CallNight
{
    public interface ICallNightService
    {
        void SubscribeUpdates();
        event Action OnCallNightHappened;
    }
}