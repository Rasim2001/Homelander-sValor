using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Infastructure;
using Infastructure.Data;
using Infastructure.StaticData.Bonfire;
using Infastructure.StaticData.Schemes;
using ModestTree;
using UnityEditor;
using UnityEngine;

namespace GoogleImporter.Parsers.Bonfire
{
    public class BonfireDataParser : IGoogleSheetParser
    {
        private readonly IImporter _importer;

        private readonly BonfireStaticData _bonfireStaticData;
        private readonly Dictionary<int, BonfireLevelData> _bonfireLevels;

        private BonfireLevelData _bonfireLevelData;

        public BonfireDataParser(IImporter importer)
        {
            _importer = importer;
            _bonfireStaticData = Resources.Load<BonfireStaticData>(AssetsPath.BonfireDataPath);
            _bonfireLevels = _bonfireStaticData.Levels
                .ToDictionary(x => x.LevelId, x => x);
        }

        public async UniTask Parse(string header, string token)
        {
            if (token.IsEmpty())
                return;

            switch (header)
            {
                case "ID":
                    int level = token.ToInt();
                    _bonfireLevelData = ForUpgradeBonfire(level);
                    break;
                case "CoinsValue":
                    _bonfireLevelData.CoinsValue = token.ToInt();
                    break;
                case "Hp":
                    _bonfireLevelData.Hp = token.ToInt();
                    break;
                case "SchemeConfigs":
                    if (!token.IsEmpty())
                        await UpdateSchemeConfig(token);
                    break;
                case "RequiredBuildings":
                    if (!token.IsEmpty())
                        await UpdateRequiredBuildings(token);
                    break;
                default:
                    Debug.Log($"Нет такого Header : {header}");
                    break;
            }
        }

        private BonfireLevelData ForUpgradeBonfire(int levelId) =>
            _bonfireLevels.GetValueOrDefault(levelId);

        private async UniTask UpdateSchemeConfig(string schemeConfigSheet)
        {
            _bonfireLevelData.SchemeConfigs = new List<SchemeConfig>();

            IGoogleSheetParser schemeConfig = new SchemeConfigDataParser(_bonfireLevelData);

            await _importer.DownloadAndParseSheet(schemeConfigSheet, schemeConfig);
        }

        private async UniTask UpdateRequiredBuildings(string requiredBuildingsSheet)
        {
            _bonfireLevelData.RequiredBuildings = new List<RequiredBuildData>();

            IGoogleSheetParser requiredBuildings = new RequiredBuildingsDataParser(_bonfireLevelData);

            await _importer.DownloadAndParseSheet(requiredBuildingsSheet, requiredBuildings);
        }
    }
}