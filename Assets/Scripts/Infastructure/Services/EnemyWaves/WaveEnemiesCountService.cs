using System.Collections.Generic;
using UnityEngine;

namespace Infastructure.Services.EnemyWaves
{
    public class WaveEnemiesCountService : IWaveEnemiesCountService
    {
        public List<GameObject> Enemies { get; set; }= new List<GameObject>();

        public int NumberOfEnemiesOnWave { get; set; }
    }
}