using System;
using Cysharp.Threading.Tasks;
using Infastructure.Data;
using Infastructure.StaticData.Bonfire;
using Infastructure.StaticData.Building;
using Infastructure.StaticData.CardsData;
using ModestTree;
using UnityEngine;

namespace GoogleImporter.Parsers.Bonfire
{
    public class RequiredBuildingsDataParser : IGoogleSheetParser
    {
        private readonly BonfireLevelData _bonfireLevelData;

        private RequiredBuildData _requiredBuildData;

        public RequiredBuildingsDataParser(BonfireLevelData bonfireLevelData) =>
            _bonfireLevelData = bonfireLevelData;

        public async UniTask Parse(string header, string token)
        {
            if (token.IsEmpty())
                return;

            switch (header)
            {
                case "BuildingTypeId":
                    BuildingTypeId buildingTypeId = Enum.Parse<BuildingTypeId>(token);

                    _requiredBuildData = new RequiredBuildData();
                    _requiredBuildData.BuildingTypeId = buildingTypeId;

                    _bonfireLevelData.RequiredBuildings.Add(_requiredBuildData);
                    break;
                case "LevelId":
                    BuildingLevelId levelId = Enum.Parse<BuildingLevelId>(token);
                    _requiredBuildData.LevelId = levelId;
                    break;
                case "CardKey":
                    CardId cardId = Enum.Parse<CardId>(token);
                    _requiredBuildData.CardKey = cardId;
                    break;
                case "Amount":
                    _requiredBuildData.Amount = token.ToInt();
                    break;
                default:
                    Debug.Log($"Нет такого Header : {header}");
                    break;
            }
        }
    }
}