using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Infastructure.StaticData.Building
{
    [CreateAssetMenu(fileName = "BuildingData", menuName = "StaticData/Building")]
    public class BuildingStaticData : SerializedScriptableObject
    {
        public BuildingTypeId BuildingTypeId;
        [Space(25)] public List<BuildingUpgradeData> BuildingsData;

        [Space(25)] public Sprite GhostSprite;

        [Header("SchemeUI")]
        public string KeyboardHint;
        public Sprite HintSprite;

        [Header("BuildingCatalogUI")]
        public Sprite CatalogItemUI;
        public string TooltipText;
    }
}