using System;
using System.Collections.Generic;
using UnityEngine;

namespace Infastructure.StaticData.EnemyCristal
{
    [Serializable]
    public class EnemyCristalConfig
    {
        public string Id;
        public Vector3 Position;
        public List<EnemyCristalData> Configs;

        public EnemyCristalConfig(string id, Vector3 position, List<EnemyCristalData> configs)
        {
            Position = position;
            Configs = configs;
            Id = id;
        }
    }
}