using System;
using System.Collections.Generic;
using UnityEngine;

namespace Infastructure.StaticData.EnemyCristal
{
    [Serializable]
    public class EnemyCristalData
    {
        [Range(0, 1)] public float PercentHealth;
        public List<EnemyCristalSpawnInfo> EnemySpawnInfos;
    }
}