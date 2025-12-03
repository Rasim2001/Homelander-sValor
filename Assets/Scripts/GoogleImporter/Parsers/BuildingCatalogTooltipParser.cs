using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Infastructure;
using Infastructure.StaticData.Building;
using Infastructure.StaticData.BuildingCatalog;
using UnityEngine;

namespace GoogleImporter.Parsers
{
    public class BuildingCatalogTooltipParser : IGoogleSheetParser
    {
        private readonly Dictionary<List<BuildingTypeId>, BuildingCatalogStaticData> _buildingCatalogs;

        private BuildingTypeId _defaultBuildingCatalog;

        public BuildingCatalogTooltipParser()
        {
            _buildingCatalogs = Resources.LoadAll<BuildingCatalogStaticData>(AssetsPath.BuildingCatalogsUIPath)
                .ToDictionary(x => x.Types, x => x);
        }

     

        public async UniTask Parse(string header, string token)
        {
            switch (header)
            {
                case "ID":
                    _defaultBuildingCatalog = Enum.Parse<BuildingTypeId>(token);
                    break;
                case "TooltipText":
                    BuildingCatalogStaticData buildingCatalogStaticData = ForCatalog(_defaultBuildingCatalog);
                    buildingCatalogStaticData.TooltipText = token;
                    break;
                default:
                    Debug.Log($"Нет такого Header : {header}");
                    break;
            }
        }

        private BuildingCatalogStaticData ForCatalog(BuildingTypeId buildingTypeId)
        {
            foreach (var catalogStaticData in _buildingCatalogs)
            {
                if (catalogStaticData.Key.Contains(buildingTypeId))
                    return catalogStaticData.Value;
            }

            return null;
        }
    }
}