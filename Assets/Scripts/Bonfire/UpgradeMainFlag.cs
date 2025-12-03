using System;
using System.Collections;
using System.Collections.Generic;
using Bonfire.Builds;
using BuildProcessManagement;
using HealthSystem;
using Infastructure;
using Infastructure.Services.BuildingRegistry;
using Infastructure.Services.PlayerProgressService;
using Infastructure.Services.PurchaseDelay;
using Infastructure.Services.SchemeSpawner;
using Infastructure.StaticData.Bonfire;
using Infastructure.StaticData.StaticDataService;
using UI.GameplayUI.BuildingCoinsUIManagement;
using Units;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace Bonfire
{
    public class UpgradeMainFlag : IUpgradeMainFlag
    {
        private readonly DiContainer _diContainer;
        private readonly IStaticDataService _staticDataService;
        private readonly IPersistentProgressService _persistentProgressService;
        private readonly IPurchaseDelayService _purchaseDelayService;
        private readonly ISchemesSpawnerService _schemesSpawnerService;
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly IBuildingRegistryService _buildingRegistryService;
        private readonly IBuildBonfire _buildBonfire;

        public int LevelIndex { get; set; } = 1;

        public event Action OnUpgradeFailed;
        public event Action OnUpgradeHappened;

        private Coroutine _schemeCoroutine;
        private ChestBonfire _chest;

        public UpgradeMainFlag(DiContainer diContainer,
            IStaticDataService staticDataService,
            IPersistentProgressService persistentProgressService,
            IPurchaseDelayService purchaseDelayService,
            ISchemesSpawnerService schemesSpawnerService,
            ICoroutineRunner coroutineRunner,
            IBuildingRegistryService buildingRegistryService,
            IBuildBonfire buildBonfire)
        {
            _diContainer = diContainer;
            _staticDataService = staticDataService;
            _persistentProgressService = persistentProgressService;
            _purchaseDelayService = purchaseDelayService;
            _schemesSpawnerService = schemesSpawnerService;
            _coroutineRunner = coroutineRunner;
            _buildingRegistryService = buildingRegistryService;
            _buildBonfire = buildBonfire;
        }


        public void Upgrade(BonfireMarker bonfireMarker, BuildingCoinsUI buildingCoinsUI)
        {
            BonfireLevelData bonfireLevelData = _staticDataService.ForUpgradeBonfire(LevelIndex);

            if (!HasUpgrade(bonfireLevelData))
            {
                OnUpgradeFailed?.Invoke();

                return;
            }

            OnUpgradeHappened?.Invoke();

            UpdateVisualBuild(bonfireLevelData, bonfireMarker,
                () => StartSchemeCoroutine(bonfireLevelData, bonfireMarker));

            RegisterHp(bonfireLevelData, bonfireMarker);
            LevelUp(bonfireMarker, bonfireLevelData);
            UpdateCoins(bonfireMarker);
            PlayCoinsAnimation(bonfireMarker, buildingCoinsUI, bonfireLevelData);
        }

        private void UpdateVisualBuild(BonfireLevelData bonfireLevelData, BonfireMarker bonfireMarker,
            Action OnCompleted)
        {
            BonfireInfo bonfireInfo = bonfireMarker.GetComponent<BonfireInfo>();

            GameObject previousBuild = bonfireInfo.NextBuild;

            bonfireInfo.NextBuild = _diContainer.InstantiatePrefab(bonfireLevelData.UpgradedBonfireObject,
                new Vector3(0, -2.85f, 0), Quaternion.identity, bonfireInfo.transform);
            bonfireInfo.NextBuild.SetActive(false);

            _buildBonfire.Build(bonfireInfo, previousBuild, OnCompleted);
        }

        public bool HasUpgrade() =>
            _staticDataService.ForUpgradeBonfire(LevelIndex) != null;


        public bool IsEnoughCoins()
        {
            BonfireLevelData bonfireLevelData = _staticDataService.ForUpgradeBonfire(LevelIndex);

            return _persistentProgressService.PlayerProgress.CoinData.IsEnoughCoins(bonfireLevelData.CoinsValue);
        }

        public bool HasUpgradeRightNow()
        {
            BonfireLevelData bonfireLevelData = _staticDataService.ForUpgradeBonfire(LevelIndex);

            if (bonfireLevelData != null)
                return HasUpgrade(bonfireLevelData);

            return false;
        }

        private bool HasUpgrade(BonfireLevelData bonfireLevelData)
        {
            if (bonfireLevelData == null)
                return false;

            List<RequiredBuildData> requiredBuildDatas = bonfireLevelData.RequiredBuildings;
            if (requiredBuildDatas == null || requiredBuildDatas.Count == 0)
                return true;

            List<BuildInfo> allBuildInfos = _buildingRegistryService.GetAllBuildInfos();

            foreach (RequiredBuildData requiredBuild in requiredBuildDatas)
            {
                bool found = false;
                foreach (BuildInfo buildInfo in allBuildInfos)
                {
                    if (buildInfo.BuildingTypeId == requiredBuild.BuildingTypeId &&
                        buildInfo.CurrentLevelId == requiredBuild.LevelId &&
                        buildInfo.CardKey == requiredBuild.CardKey)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                    return false;
            }

            return true;
        }

        private void RegisterHp(BonfireLevelData bonfireLevelData, BonfireMarker bonfireMarker)
        {
            IHealth health = bonfireMarker.GetComponentInChildren<IHealth>();
            health.Initialize(bonfireLevelData.Hp);
        }

        private void UpdateCoins(BonfireMarker bonfireMarker)
        {
            BonfireCoinsUIRegistrator bonfireCoinsUIRegistrator =
                bonfireMarker.GetComponent<BonfireCoinsUIRegistrator>();

            bonfireCoinsUIRegistrator.UpdateCoinsUI(LevelIndex);
        }

        private void PlayCoinsAnimation(BonfireMarker bonfireMarker, BuildingCoinsUI buildingCoinsUI,
            BonfireLevelData bonfireLevelData)
        {
            buildingCoinsUI.PlaySpendAnimation(bonfireLevelData.CoinsValue, bonfireMarker.transform);
            buildingCoinsUI.Hide();
        }

        private void LevelUp(BonfireMarker bonfireMarker, BonfireLevelData bonfireLevelData)
        {
            UniqueId uniqueId = bonfireMarker.GetComponent<UniqueId>();

            _persistentProgressService.PlayerProgress.CoinData.Spend(bonfireLevelData.CoinsValue);
            _purchaseDelayService.AddDelay(uniqueId.Id);

            LevelIndex++;
        }

        private void StartSchemeCoroutine(BonfireLevelData bonfireLevelData, BonfireMarker bonfireMarker)
        {
            BonfireInfo bonfireInfo = bonfireMarker.GetComponent<BonfireInfo>();

            ISetParent gameObjectToSetParent = bonfireInfo.NextBuild.GetComponentInChildren<ISetParent>();
            gameObjectToSetParent?.SetParent(bonfireMarker.transform);

            if (_schemeCoroutine != null)
            {
                _coroutineRunner.StopCoroutine(_schemeCoroutine);
                _schemeCoroutine = null;
            }

            _schemeCoroutine =
                _coroutineRunner.StartCoroutine(StartCreateSchemeCoroutine(bonfireLevelData, bonfireMarker));
        }

        private IEnumerator StartCreateSchemeCoroutine(BonfireLevelData bonfireLevelData, BonfireMarker bonfireMarker)
        {
            InitChest(bonfireMarker);

            _chest.Show();
            Vector3 chestPosition = _chest.transform.position + new Vector3(0, 0.25f, 0);

            yield return new WaitForSeconds(0.5f);

            _schemesSpawnerService.CreateShemesByMainflag(bonfireLevelData, chestPosition, () => _chest.Hide());
        }

        private void InitChest(BonfireMarker bonfireMarker)
        {
            if (_chest == null)
                _chest = bonfireMarker.GetComponentInChildren<ChestBonfire>();
        }
    }
}