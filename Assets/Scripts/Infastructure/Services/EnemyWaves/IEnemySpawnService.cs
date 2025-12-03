using UI.GameplayUI;
using UnityEngine;

namespace Infastructure.Services.EnemyWaves
{
    public interface IEnemySpawnService
    {
        void StartSpawnEnemies(int levelId, int waveId);
        void Initialize(Transform playerTransform, WaveNotificatorUI waveNotificatorUI);
        bool EnemyWaveFinished();
        int WavePassedWaveId { get; }
    }
}