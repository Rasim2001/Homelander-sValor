using System.Collections.Generic;
using UnityEngine;

namespace Infastructure.StaticData.Tutorial
{
    [CreateAssetMenu(fileName = "TutorialData", menuName = "StaticData/TutorialData")]
    public class TutorialStaticData : ScriptableObject
    {
        public List<TutorialData> TutorialObjects;
    }
}