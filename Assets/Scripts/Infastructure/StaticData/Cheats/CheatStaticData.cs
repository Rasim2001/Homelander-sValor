using System;
using System.Collections.Generic;
using Infastructure.StaticData.Unit;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Infastructure.StaticData.Cheats
{
    [CreateAssetMenu(fileName = "CheatStaticData", menuName = "StaticData/CheatStaticData")]
    public class CheatStaticData : ScriptableObject
    {
        public int LevelWaveId;

        public bool SkipCutScene;
        public bool GetSchemes;

        public List<UnitCheatInfo> UnitCheatInfos;
    }
}

[Serializable]
public class UnitCheatInfo
{
    [FoldoutGroup("UnitCheat")] public UnitTypeId UnitTypeId;
    [FoldoutGroup("UnitCheat")] public int Amount;
}