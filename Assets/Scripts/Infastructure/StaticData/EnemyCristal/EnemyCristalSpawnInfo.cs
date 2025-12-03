using System;
using Enemy;

namespace Infastructure.StaticData.EnemyCristal
{
    [Serializable]
    public class EnemyCristalSpawnInfo
    {
        public EnemyTypeId EnemyTypeId;
        public int Amount;
    }
}