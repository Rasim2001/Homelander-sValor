using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Infastructure.StaticData.Tutorial
{
    [CreateAssetMenu(fileName = "TutorialData", menuName = "StaticData/TutorialData")]
    public class TutorialStaticData : SerializedScriptableObject
    {
        public List<TutorialData> TutorialObjects;

        public Dictionary<TutorialEventData, string> Infos;
    }


    public enum TutorialEventData
    {
        MovementEvent = 0,
        CutSceneEvent = 1,
        AccelerationEvent = 2,
        MainFlagStartBuildEvent = 3,
        MainFlagFinishBuildEvent = 4,
        BarricadeStartBuildEvent = 5,
        BarricadeFinishBuildEvent = 6,
        BarricadeBuildingEvent = 7,
        AttackStartEvent = 8,
    }
}