using System;
using Cysharp.Threading.Tasks;
using Enemy;
using Infastructure.Data;
using Infastructure.StaticData.Bonfire;
using Infastructure.StaticData.Building;
using Infastructure.StaticData.Schemes;
using UnityEngine;

namespace GoogleImporter.Parsers.Bonfire
{
    public class SchemeConfigDataParser : IGoogleSheetParser
    {
        private readonly BonfireLevelData _bonfireLevelData;
        private SchemeConfig _schemeConfig;

        public SchemeConfigDataParser(BonfireLevelData bonfireLevelData) =>
            _bonfireLevelData = bonfireLevelData;


        public async UniTask Parse(string header, string token)
        {
            switch (header)
            {
                case "BuildingTypeId":
                    BuildingTypeId buildingTypeId = Enum.Parse<BuildingTypeId>(token);
                    _schemeConfig = new SchemeConfig();
                    _schemeConfig.BuildingTypeId = buildingTypeId;
                    _bonfireLevelData.SchemeConfigs.Add(_schemeConfig);
                    break;
                case "Amount":
                    _schemeConfig.Amount = token.ToInt();
                    break;
                default:
                    Debug.Log($"Нет такого Header : {header}");
                    break;
            }
        }
    }
}