using System.Collections.Generic;
using Infastructure.StaticData.Building;
using UnityEngine;

namespace Infastructure.StaticData.BuildingCatalog
{
    [CreateAssetMenu(fileName = "CatalogData", menuName = "StaticData/BuildingCatalogData")]
    public class BuildingCatalogStaticData : ScriptableObject
    {
        public List<BuildingTypeId> Types;
        public Sprite CatalogSprite;
        public string TooltipText;
    }
}