using System.Collections.Generic;
using UnityEngine;

namespace Infastructure.StaticData.WaveOfEnemies
{
    [CreateAssetMenu(fileName = "WaveLevelData", menuName = "StaticData/WaveLevelData")]
    public class WaveLevelStaticData : ScriptableObject
    {
        public int LevelId;
        public List<WaveStaticData> Waves;
    }
}