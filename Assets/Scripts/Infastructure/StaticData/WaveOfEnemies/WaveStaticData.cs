using System;
using System.Collections.Generic;
using Enemy;
using Infastructure.StaticData.DayCycle;
using UnityEngine;

namespace Infastructure.StaticData.WaveOfEnemies
{
    [CreateAssetMenu(fileName = "WaveData", menuName = "StaticData/Wave")]
    public class WaveStaticData : ScriptableObject
    {
        public int WaveId;

        public int TimeWaitOfDay;
        public int TimeWaitOfNight;
        public int TimeBetweenMicroWaves;
        public int PassedWaveCoins;
        public int PreNightPreparationTimeInSeconds;

        public MicroWavesInfo MicroWavesInfo;
        public DayCyclePresetStaticData DayCyclePreset;
    }

    [Serializable]
    public class MicroWavesInfo
    {
        public List<EnemyWaveInfo> Waves;
    }


    [Serializable]
    public class EnemyWaveInfo
    {
        public EnemyTypeId EnemyTypeId;
        public int Amount;
        public float TimeBetweenSpawnEnemy;
    }
}