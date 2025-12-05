using System.Collections.Generic;
using System.Linq;
using Bonfire;
using BuildProcessManagement;
using Infastructure.Services.BuildingRegistry;
using Infastructure.Services.ECSInput;
using Infastructure.Services.PlayerProgressService;
using Infastructure.StaticData.Bonfire;
using Infastructure.StaticData.Schemes;
using Infastructure.StaticData.StaticDataService;
using Infastructure.StaticData.Windows;
using UI.Windows.Mainflag;
using UnityEngine;
using Zenject;

namespace Infastructure.Factories.GameFactories
{
    public class GameUIFactory : IGameUIFactory
    {
        private readonly DiContainer _diContainer;
        private readonly IStaticDataService _staticData;
        private readonly IEcsWatchersService _ecsWatchersService;
        private readonly IUpgradeMainFlag _upgradeMainFlag;
        private readonly IBuildingRegistryService _buildingRegistryService;
        private readonly IPersistentProgressService _persistentProgressService;

        private GameObject _uiRoot;


        public GameUIFactory(
            DiContainer diContainer,
            IStaticDataService staticDataService,
            IEcsWatchersService ecsWatchersService,
            IUpgradeMainFlag upgradeMainFlag,
            IBuildingRegistryService buildingRegistryService,
            IPersistentProgressService persistentProgressService)
        {
            _diContainer = diContainer;
            _staticData = staticDataService;
            _ecsWatchersService = ecsWatchersService;
            _upgradeMainFlag = upgradeMainFlag;
            _buildingRegistryService = buildingRegistryService;
            _persistentProgressService = persistentProgressService;
        }

        public void CreateMenuRootUI()
        {
            GameObject prefab = Resources.Load<GameObject>(UIAssetPath.GameMenuUIRootPath);
            _uiRoot = _diContainer.InstantiatePrefab(prefab);
        }

        public void CreateTaskBookWindow(WindowId windowId)
        {
            BonfireLevelData bonfireLevelData = _staticData.ForUpgradeBonfire(_upgradeMainFlag.LevelIndex);
            if (bonfireLevelData == null)
                return;

            WindowConfig windowConfig = _staticData.ForWindow(windowId);
            GameObject window = _diContainer.InstantiatePrefab(windowConfig.Prefab, _uiRoot.transform);

            List<RequiredBuildData> requiredBuildDatas = bonfireLevelData.RequiredBuildings;
            List<SchemeConfig> schemeConfigs = bonfireLevelData.SchemeConfigs;

            List<BuildInfo> allBuildInfos = _buildingRegistryService.GetAllBuildInfos();
            TaskBookWindow mainFlagWindow = window.GetComponent<TaskBookWindow>();

            foreach (RequiredBuildData requiredBuildData in requiredBuildDatas)
            {
                int completedLocalTasks = GetLocalCompletedTask(allBuildInfos, requiredBuildData);
                int allLocalTasks = requiredBuildData.Amount;


                RequiredBuildIconData requiredBuildIconData = _staticData.ForRequiredBuilding(requiredBuildData);

                Sprite icon = completedLocalTasks / allLocalTasks == 1
                    ? requiredBuildIconData.Icon
                    : requiredBuildIconData.DisabledIcon;

                mainFlagWindow.CreateTask(icon, completedLocalTasks, allLocalTasks);
            }

            foreach (SchemeConfig schemeConfig in schemeConfigs)
            {
                Sprite icon = _staticData.ForSchemeIcon(schemeConfig.BuildingTypeId);
                mainFlagWindow.CreateAward(icon, schemeConfig.Amount);
            }

            CoinsIconData coinsIconData = _staticData.ForCoinsIcon();
            int playerCoins = _persistentProgressService.PlayerProgress.CoinData.NumberOfCoins;
            int requiredCoins = bonfireLevelData.CoinsValue;

            Sprite coinIcon = playerCoins > requiredCoins ? coinsIconData.Icon : coinsIconData.DisabledIcon;
            mainFlagWindow.CreateTask(coinIcon, playerCoins, requiredCoins);
            mainFlagWindow.SetMainFlagLevel(bonfireLevelData.LevelId);

            Sprite mainFlagIcon = _staticData.ForMainFlagIcon(bonfireLevelData.LevelId);
            mainFlagWindow.SetMainFlagIcon(mainFlagIcon);

            _ecsWatchersService.RegisterWatchers(window);
        }

        public GameObject CreateGameSettingsWindow(WindowId windowId)
        {
            WindowConfig windowConfig = _staticData.ForWindow(windowId);
            GameObject window = _diContainer.InstantiatePrefab(windowConfig.Prefab, _uiRoot.transform);

            _ecsWatchersService.RegisterWatchers(window);

            return window;
        }
        
        public GameObject CreateTutorialWindow(WindowId windowId)
        {
            WindowConfig windowConfig = _staticData.ForWindow(windowId);
            GameObject window = _diContainer.InstantiatePrefab(windowConfig.Prefab, _uiRoot.transform);

            return window;
        }


        public void CreateFinishTutorialWindow(WindowId windowId)
        {
            WindowConfig windowConfig = _staticData.ForWindow(windowId);
            _diContainer.InstantiatePrefab(windowConfig.Prefab, _uiRoot.transform);
        }

        public void CreateWinWindow(WindowId windowId)
        {
            WindowConfig windowConfig = _staticData.ForWindow(windowId);
            _diContainer.InstantiatePrefab(windowConfig.Prefab, _uiRoot.transform);
        }

        public void CreateDefeatWindow(WindowId windowId)
        {
            WindowConfig windowConfig = _staticData.ForWindow(windowId);
            _diContainer.InstantiatePrefab(windowConfig.Prefab, _uiRoot.transform);
        }

        public GameObject CreateCardsWindow(WindowId windowId)
        {
            WindowConfig windowConfig = _staticData.ForWindow(windowId);
            GameObject prefab = _diContainer.InstantiatePrefab(windowConfig.Prefab, _uiRoot.transform);

            _ecsWatchersService.RegisterWatchers(prefab);

            return prefab;
        }

        private int GetAllCompletedTask(List<RequiredBuildData> requiredBuildDatas, List<BuildInfo> allBuildInfos)
        {
            int amount = 0;

            foreach (RequiredBuildData requiredBuild in requiredBuildDatas)
            {
                int amountOfTask = GetLocalCompletedTask(allBuildInfos, requiredBuild);

                if (amountOfTask == requiredBuild.Amount)
                    amount++;
            }

            return amount;
        }

        private int GetLocalCompletedTask(List<BuildInfo> allBuildInfos, RequiredBuildData requiredBuild)
        {
            return allBuildInfos.Count(x => x.BuildingTypeId == requiredBuild.BuildingTypeId &&
                                            x.CurrentLevelId == requiredBuild.LevelId &&
                                            x.CardKey == requiredBuild.CardKey);
        }
    }
}