using System.Collections.Generic;
using UnityEngine;

namespace Infastructure.Services.EnemyWaves
{
    public interface IWaveEnemiesCountService
    {
        int NumberOfEnemiesOnWave { get; set; }
        List<GameObject> Enemies { get; set; }
    }
}