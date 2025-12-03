using System;
using CameraManagement;

namespace Infastructure.Services.CameraFocus
{
    public interface ICameraFocusService
    {
        void ShowMainFlagDestruction();
        void Initialize(CinemachineFollow cinemachineFollow);
        bool PlayerDefeated { get; set; }
        event Action OnDefeatHappened;
    }
}