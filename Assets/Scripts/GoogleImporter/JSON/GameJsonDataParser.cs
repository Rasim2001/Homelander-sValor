using System.Collections.Generic;
using System.Linq;
using BuildProcessManagement.SpawnMarker;
using Cysharp.Threading.Tasks;
using Infastructure;
using Infastructure.StaticData;
using Infastructure.StaticData.Building;
using Infastructure.StaticData.EnemyCristal;
using Infastructure.StaticData.Forest;
using Infastructure.StaticData.RecourceElements;
using Infastructure.StaticData.VagabondCampManagement;
using UnityEngine;

namespace GoogleImporter.JSON
{
    public class GameJsonDataParser : IGoogleSheetParser
    {
        private readonly JsonSaveLoader _jsonSaveLoader;
        private readonly GameStaticData _gameStaticData;

        public GameJsonDataParser(JsonSaveLoader jsonSaveLoader)
        {
            _jsonSaveLoader = jsonSaveLoader;

            _gameStaticData = Resources.Load<GameStaticData>(AssetsPath.GameDataPath);
        }

        public async UniTask Parse(string header, string token)
        {
            switch (header)
            {
                case "GameData":
                    GameJsonData gameJsonData = _jsonSaveLoader.Load(token);
                    UpdateGameData(gameJsonData);
                    UpdateGameDataScene(gameJsonData);
                    break;

                default:
                    Debug.Log($"Нет такого Header : {header}");
                    break;
            }
        }

        private void UpdateGameData(GameJsonData gameJsonData)
        {
            _gameStaticData.BuildingSpawners = new List<BuildingSpawnerData>(gameJsonData.BuildingSpawners);
            _gameStaticData.ResourceSpawners = new List<ResourceSpawnerData>(gameJsonData.ResourceSpawners);
            _gameStaticData.ForestSides = new List<ForestData>(gameJsonData.ForestSides);
            _gameStaticData.VagabondCampDatas = new List<VagabondCampData>(gameJsonData.VagabondCampDatas);
            _gameStaticData.EnemyCristalConfigs =
                new List<EnemyCristalConfig>(gameJsonData.EnemyCristalConfigs);
        }

        private void UpdateGameDataScene(GameJsonData gameJsonData)
        {
            GameDataMarker gameDataMarker = Object.FindObjectOfType<GameDataMarker>();

            gameDataMarker.transform
                .Cast<Transform>()
                .ToList()
                .ForEach(child => Object.DestroyImmediate(child.gameObject));

            InitBuilding(gameJsonData, gameDataMarker);
            InitResources(gameJsonData, gameDataMarker);
            InitForest(gameJsonData, gameDataMarker);
            InitVagabondCamps(gameJsonData, gameDataMarker);
            InitEnemyCristalConfig(gameJsonData, gameDataMarker);
        }

        private void InitBuilding(GameJsonData gameJsonData, GameDataMarker gameDataMarker)
        {
            GameObject building = new GameObject("Building");
            building.transform.SetParent(gameDataMarker.transform);

            InstantiateBuildingData(building, gameJsonData.BuildingSpawners);
        }

        private void InitResources(GameJsonData gameJsonData, GameDataMarker gameDataMarker)
        {
            GameObject resources = new GameObject("Resources");
            resources.transform.SetParent(gameDataMarker.transform);

            InstantiateResourcesData(resources, gameJsonData.ResourceSpawners);
        }

        private void InitForest(GameJsonData gameJsonData, GameDataMarker gameDataMarker)
        {
            GameObject forest = new GameObject("Forest");
            forest.transform.SetParent(gameDataMarker.transform);

            InstantiateForestData(forest, gameJsonData.ForestSides);
        }

        private void InitVagabondCamps(GameJsonData gameJsonData, GameDataMarker gameDataMarker)
        {
            GameObject vagabondCamps = new GameObject("VagabondCamps");
            vagabondCamps.transform.SetParent(gameDataMarker.transform);

            InstantiateVagabondCamps(vagabondCamps, gameJsonData.VagabondCampDatas);
        }

        private void InitEnemyCristalConfig(GameJsonData gameJsonData, GameDataMarker gameDataMarker)
        {
            GameObject enemyCristalCamp = new GameObject("EnemyCristals");
            enemyCristalCamp.transform.SetParent(gameDataMarker.transform);

            InstantiateEnemyCristalConfig(enemyCristalCamp, gameJsonData.EnemyCristalConfigs);
        }

        private void InstantiateBuildingData(GameObject building, List<BuildingSpawnerData> buildingSpawners)
        {
            foreach (BuildingSpawnerData buildingSpawnerData in buildingSpawners)
            {
                GameObject buildingMarker = new GameObject("BuildingMarker");
                buildingMarker.transform.SetParent(building.transform);

                BuildingSpawnerMarker buildingSpawnerMarker = buildingMarker.AddComponent<BuildingSpawnerMarker>();

                buildingSpawnerMarker._buildingTypeId = buildingSpawnerData.BuildingTypeId;
                buildingSpawnerMarker.transform.position = buildingSpawnerData.Position;
                buildingSpawnerMarker.UniqueId = buildingSpawnerData.UniqueId;
            }
        }

        private void InstantiateResourcesData(GameObject building, List<ResourceSpawnerData> resourceSpawners)
        {
            foreach (ResourceSpawnerData resourceSpawnerData in resourceSpawners)
            {
                GameObject resourceMarker = new GameObject("ResourceMarker");
                resourceMarker.transform.SetParent(building.transform);

                ResourceSpawnerMarker resourceSpawnerMarker = resourceMarker.AddComponent<ResourceSpawnerMarker>();

                resourceSpawnerMarker.ResourceId = resourceSpawnerData.ResourceId;
                resourceSpawnerMarker.transform.position = resourceSpawnerData.Position;
                resourceSpawnerMarker.UniqueId = resourceSpawnerData.UniqueId;
            }
        }

        private void InstantiateForestData(GameObject building, List<ForestData> forestSides)
        {
            foreach (ForestData forestSpawnerData in forestSides)
            {
                GameObject forestMarker = new GameObject("ForestMarker");
                forestMarker.transform.SetParent(building.transform);

                ForestMarker forestSpawnerMarker = forestMarker.AddComponent<ForestMarker>();
                BoxCollider2D boxCollider = forestMarker.AddComponent<BoxCollider2D>();

                forestSpawnerMarker.transform.position = forestSpawnerData.Position;
                boxCollider.size = forestSpawnerData.ColliderSize;
                boxCollider.offset = forestSpawnerData.ColliderOffset;
            }
        }

        private void InstantiateVagabondCamps(GameObject vagabondCamps, List<VagabondCampData> vagabondCampDatas)
        {
            foreach (VagabondCampData vagabondCampSpawnerData in vagabondCampDatas)
            {
                GameObject vagabondCampMarker = new GameObject("VagabondCamp");
                vagabondCampMarker.transform.SetParent(vagabondCamps.transform);

                VagabondCampMarker campMarker = vagabondCampMarker.AddComponent<VagabondCampMarker>();

                campMarker.transform.position = vagabondCampSpawnerData.Position;
                campMarker.UniqueId = vagabondCampSpawnerData.UniqueId;
            }
        }

        private void InstantiateEnemyCristalConfig(GameObject enemyCristalCamp,
            List<EnemyCristalConfig> enemyCristalConfigs)
        {
            foreach (EnemyCristalConfig enemyCristalConfig in enemyCristalConfigs)
            {
                GameObject cristal = new GameObject("Cristal");
                cristal.transform.SetParent(enemyCristalCamp.transform);

                EnemyCristalMarker cristalMarker = cristal.AddComponent<EnemyCristalMarker>();

                cristalMarker.transform.position = enemyCristalConfig.Position;
                cristalMarker.Configs = new List<EnemyCristalData>(enemyCristalConfig.Configs);
                cristalMarker.UniqueId = enemyCristalConfig.Id;
            }
        }
    }
}