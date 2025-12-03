using System;
using Infastructure.StaticData.WaveOfEnemies;
using UnityEngine;

namespace Infastructure.StaticData.Enemy
{
    [Serializable]
    public class EnemyCampData
    {
        public string UniqueId;

        public int Hp;
        public MicroWavesInfo MicroWaveCamp;
        public Vector3 Position;

        public EnemyCampData(Vector3 position, MicroWavesInfo microWaveCamp, int hp, string uniqueId)
        {
            Position = position;
            MicroWaveCamp = microWaveCamp;
            Hp = hp;
            UniqueId = uniqueId;
        }
    }
}