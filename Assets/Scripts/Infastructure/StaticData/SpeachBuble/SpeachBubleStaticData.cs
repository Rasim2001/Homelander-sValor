using System.Collections.Generic;
using Infastructure.StaticData.SpeachBuble.Player;
using Infastructure.StaticData.SpeachBuble.Units;
using UnityEngine;

namespace Infastructure.StaticData.SpeachBuble
{
    [CreateAssetMenu(fileName = "SpeachBubleData", menuName = "StaticData/SpeachBubleData")]
    public class SpeachBubleStaticData : ScriptableObject
    {
        public List<SpeachBubleConfig> PlayerConfigs;
        public List<SpeachBubleOrderConfig> UnitConfigs;
        public List<SpeachBubleHomelessOrderConfig> HomelessConfigs;
    }
}