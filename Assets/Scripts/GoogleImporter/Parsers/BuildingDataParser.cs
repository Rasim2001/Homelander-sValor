using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Infastructure;
using Infastructure.Data;
using Infastructure.StaticData.Building;
using UnityEngine;

namespace GoogleImporter.Parsers
{
    public class BuildingDataParser : IGoogleSheetParser
    {
        private readonly Dictionary<BuildingTypeId, BuildingStaticData> _buildings;

        private BuildingUpgradeData _buildingUpgradeData;
        private BuildingTypeId _buildingType;

        public BuildingDataParser()
        {
            _buildings = Resources.LoadAll<BuildingStaticData>(AssetsPath.BuildingsDataPath)
                .ToDictionary(x => x.BuildingTypeId, x => x);
        }


        public async UniTask Parse(string header, string token)
        {
            switch (header)
            {
                case "ID":
                    _buildingType = Enum.Parse<BuildingTypeId>(token);
                    break;
                case "LevelId":
                    BuildingLevelId levelId = Enum.Parse<BuildingLevelId>(token);
                    _buildingUpgradeData = ForBuilding(_buildingType, levelId);
                    break;
                case "HP":
                    _buildingUpgradeData.HP = token.ToInt();
                    break;
                case "GridSize":
                    _buildingUpgradeData.GridSizeX = token.ToInt();
                    break;
                case "CoinsValue":
                    _buildingUpgradeData.CoinsValue = token.ToInt();
                    break;
                case "BuildingTime":
                    _buildingUpgradeData.BuildingTime = token.ToInt();
                    break;
                default:
                    Debug.Log($"Нет такого Header : {header}");
                    break;
            }
        }

        private BuildingUpgradeData ForBuilding(BuildingTypeId buildingTypeId, BuildingLevelId levelId)
        {
            if (_buildings.TryGetValue(buildingTypeId, out BuildingStaticData buildingData))
            {
                foreach (BuildingUpgradeData data in buildingData.BuildingsData)
                {
                    if (data.LevelId == levelId)
                        return data;
                }
            }

            return null;
        }
    }
}