using System;
using DayCycle;
using UI;
using UI.GameplayUI;

namespace Infastructure.Services.EnemyWaves
{
    public interface IEnemyWavesService
    {
        void Initialize(WavesProgressBar wavesProgressBar, DayCycleUpdater dayCycleUpdater);
        void StartWaveCycle();
        void FreezTimeEditor();
        void UnFreezTimeEditor();
        void ForceNight();
        void ForceDayEditor();
    }
}