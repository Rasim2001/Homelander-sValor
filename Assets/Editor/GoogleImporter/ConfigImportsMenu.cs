using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using GoogleImporter;
using GoogleImporter.JSON;
using GoogleImporter.Parsers;
using GoogleImporter.Parsers.Bonfire;
using Infastructure;
using Infastructure.StaticData;
using Infastructure.StaticData.Building;
using Infastructure.StaticData.EnemyCristal;
using Infastructure.StaticData.Forest;
using Infastructure.StaticData.RecourceElements;
using Infastructure.StaticData.VagabondCampManagement;
using Infastructure.StaticData.WaveOfEnemies;
using UnityEditor;
using UnityEngine;

namespace Editor.GoogleImporter
{
    public static class ConfigImportsMenu
    {
        private const string SPREADSHEET_PATH = "1VnAkd5YvWKWGpcc_5Bza5y47pSVzRDGg-vMW8iGJp2o";
        private const string CREDENTIALS_PATH = "homelendervalor-567620a1acbe.json";

        private const string LOCAL_EXCEL_PATH = "Resources/Excel/HomelenderValor.xlsx";

        private const string BUILDING_FLAG_TOOLTIPS = "BuildingFlagTooltips";
        private const string UNIT_TYPE_TOOLTIPS = "UnitTypeTooltips";
        private const string BUILDING_CATALOG_TOOLTIPS = "BuildingCatalogTooltips";
        private const string BUILDING_TOOLTIPS = "BuildingTooltips";
        private const string BUILDING_DATA = "BuildingData";
        private const string UNIT_DATA = "UnitData";
        private const string WAVE_DATA = "WaveData";
        private const string ENEMY_DATA = "EnemyData";
        private const string BONFIRE_DATA = "BonfireData";
        private const string PLAYER_DATA = "PlayerData";
        private const string VAGABOND_CAMP_DATA = "VagabondCampData";


        private const string JSON_DATA = "JsonData";


        private static Dictionary<int, WaveLevelStaticData> _waveLevels;

        [MenuItem("GoogleSheet/Import Remote Settings")]
        private static async void LoadRemoteItemsSettings()
        {
            IImporter sheetImporter = new GoogleSheetsImporter(CREDENTIALS_PATH, SPREADSHEET_PATH);

            await LoadSettings(sheetImporter);
        }


        [MenuItem("GoogleSheet/Import Local Settings")]
        private static async void LoadLocalItemsSettings()
        {
            IImporter excelImporter = new ExcelImporter(LOCAL_EXCEL_PATH);

            await LoadSettings(excelImporter);
        }


        [MenuItem("GoogleSheet/Update JSON Settings")]
        private static async void UpdateJsonItemsSettings()
        {
            IImporter googleImporter = new GoogleSheetsImporter(CREDENTIALS_PATH, SPREADSHEET_PATH);

            await UpdateJsonSettings(googleImporter);
        }

        [MenuItem("GoogleSheet/Load JSON Settings")]
        private static async void LoadJsonItemsSettings()
        {
            IImporter googleImporter = new GoogleSheetsImporter(CREDENTIALS_PATH, SPREADSHEET_PATH);

            await LoadJsonSettings(googleImporter);
        }

        private static async UniTask UpdateJsonSettings(IImporter excelImporter)
        {
            GameStaticData gameStaticData = Resources.Load<GameStaticData>(AssetsPath.GameDataPath);

            GameJsonData gameJsonData = new GameJsonData
            {
                ResourceSpawners = new List<ResourceSpawnerData>(gameStaticData.ResourceSpawners),
                ForestSides = new List<ForestData>(gameStaticData.ForestSides),
                VagabondCampDatas = new List<VagabondCampData>(gameStaticData.VagabondCampDatas),
                EnemyCristalConfigs = new List<EnemyCristalConfig>(gameStaticData.EnemyCristalConfigs),
                BuildingSpawners = new List<BuildingSpawnerData>(gameStaticData.BuildingSpawners)
            };

            JsonSaveLoader jsonSaveLoader = new JsonSaveLoader();
            string json = jsonSaveLoader.ConvertToJson(gameJsonData);

            await excelImporter.UpdateRange(JSON_DATA, "A2", new List<IList<object>>
            {
                new List<object> { json }
            });

            Debug.Log("Все прошло успешно");
        }

        private static async UniTask LoadJsonSettings(IImporter googleImporter)
        {
            JsonSaveLoader jsonSaveLoader = new JsonSaveLoader();
            IGoogleSheetParser jsonParser = new GameJsonDataParser(jsonSaveLoader);

            await googleImporter.DownloadAndParseSheet(JSON_DATA, jsonParser);

            foreach (ScriptableObject asset in Resources.LoadAll<ScriptableObject>("StaticData"))
                EditorUtility.SetDirty(asset);

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();

            Debug.Log("Все прошло успешно");
        }


        private static async Task LoadSettings(IImporter excelImporter)
        {
            _waveLevels = Resources.LoadAll<WaveLevelStaticData>(AssetsPath.WavesDataPath)
                .ToDictionary(x => x.LevelId, x => x);

            for (int i = 0; i < _waveLevels.Count; i++)
            {
                IGoogleSheetParser waveData = new WaveDataParser(excelImporter, _waveLevels[i].Waves);
                string sheetName = WAVE_DATA + $"_{i}";
                await excelImporter.DownloadAndParseSheet(sheetName, waveData);
            }

            IGoogleSheetParser buildingTypeParser = new BuildingFlagTooltipParser();
            IGoogleSheetParser unitTypeParser = new UnitTypeTooltipParser();
            IGoogleSheetParser buildingCatalogParser = new BuildingCatalogTooltipParser();
            IGoogleSheetParser buildingCatalogItemParser = new BuildingCatalogItemTooltipParser();
            IGoogleSheetParser buildingData = new BuildingDataParser();
            IGoogleSheetParser unitData = new UnitDataParser();

            IGoogleSheetParser enemyData = new EnemyDataParser();
            IGoogleSheetParser bonfireData = new BonfireDataParser(excelImporter);
            IGoogleSheetParser playerData = new PlayerDataParser();
            IGoogleSheetParser vagabondCampData = new VagabondCampParser();

            await excelImporter.DownloadAndParseSheet(BUILDING_FLAG_TOOLTIPS, buildingTypeParser);
            await excelImporter.DownloadAndParseSheet(UNIT_TYPE_TOOLTIPS, unitTypeParser);
            await excelImporter.DownloadAndParseSheet(BUILDING_CATALOG_TOOLTIPS, buildingCatalogParser);
            await excelImporter.DownloadAndParseSheet(BUILDING_TOOLTIPS, buildingCatalogItemParser);
            await excelImporter.DownloadAndParseSheet(BUILDING_DATA, buildingData);
            await excelImporter.DownloadAndParseSheet(UNIT_DATA, unitData);

            await excelImporter.DownloadAndParseSheet(ENEMY_DATA, enemyData);
            await excelImporter.DownloadAndParseSheet(BONFIRE_DATA, bonfireData);
            await excelImporter.DownloadAndParseSheet(PLAYER_DATA, playerData);
            await excelImporter.DownloadAndParseSheet(VAGABOND_CAMP_DATA, vagabondCampData);

            foreach (ScriptableObject asset in Resources.LoadAll<ScriptableObject>("StaticData"))
                EditorUtility.SetDirty(asset);

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();

            Debug.Log("Все прошло успешно");
        }
    }
}