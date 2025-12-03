using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Infastructure;
using Infastructure.StaticData.Building;
using UnityEngine;

namespace GoogleImporter.Parsers
{
    public class BuildingCatalogItemTooltipParser : IGoogleSheetParser
    {
        private readonly Dictionary<BuildingTypeId, BuildingStaticData> _buildings;

        private BuildingStaticData _buildingStaticData;
        private BuildingTypeId _buildingTypeId;

        public BuildingCatalogItemTooltipParser()
        {
            _buildings = Resources.LoadAll<BuildingStaticData>(AssetsPath.BuildingsDataPath)
                .ToDictionary(x => x.BuildingTypeId, x => x);
        }

        public ScriptableObject GetScriptableObject() =>
            _buildingStaticData;

        public async UniTask Parse(string header, string token)
        {
            switch (header)
            {
                case "ID":
                    _buildingTypeId = Enum.Parse<BuildingTypeId>(token);
                    _buildingStaticData = ForBuilding(_buildingTypeId);
                    break;
                case "TooltipText":
                    _buildingStaticData.TooltipText = token;
                    break;
                case "KeyboardHint":
                    _buildingStaticData.KeyboardHint = token;
                    break;
                default:
                    Debug.Log($"Нет такого Header : {header}");
                    break;
            }
        }

        private BuildingStaticData ForBuilding(BuildingTypeId buildingTypeId) =>
            _buildings.GetValueOrDefault(buildingTypeId);
    }
}