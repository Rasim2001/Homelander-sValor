using System;
using System.Collections.Generic;
using System.Linq;
using BuildProcessManagement.WorkshopBuilding;
using Enemy;
using Enemy.Effects;
using Infastructure.StaticData.Bonfire;
using Infastructure.StaticData.Building;
using Infastructure.StaticData.BuildingCatalog;
using Infastructure.StaticData.CardsData;
using Infastructure.StaticData.Cheats;
using Infastructure.StaticData.Coins;
using Infastructure.StaticData.DefaultMaterial;
using Infastructure.StaticData.EffectsUI;
using Infastructure.StaticData.Enemy;
using Infastructure.StaticData.EnemyCristal;
using Infastructure.StaticData.Forest;
using Infastructure.StaticData.Lights;
using Infastructure.StaticData.Matryoshka;
using Infastructure.StaticData.Player;
using Infastructure.StaticData.RecourceElements;
using Infastructure.StaticData.Schemes;
using Infastructure.StaticData.SpeachBuble;
using Infastructure.StaticData.SpeachBuble.Player;
using Infastructure.StaticData.SpeachBuble.Units;
using Infastructure.StaticData.Tutorial;
using Infastructure.StaticData.Unit;
using Infastructure.StaticData.VagabondCampManagement;
using Infastructure.StaticData.WaveOfEnemies;
using Infastructure.StaticData.Windows;
using Infastructure.StaticData.Workshop;
using Infrastructure.StaticData.Tooltip;
using Player.Orders;
using UnityEngine;

namespace Infastructure.StaticData.StaticDataService
{
    public class StaticDataService : IStaticDataService
    {
        private Dictionary<WindowId, WindowConfig> _windows;
        private Dictionary<SpeachBubleId, SpeachBubleConfig> _playerSpeachBubleConfigs;
        private Dictionary<List<BuildingTypeId>, BuildingCatalogStaticData> _buildingCatalogs;
        private Dictionary<OrderID, SpeachBubleOrderConfig> _unitsSpeachBubleConfigs;
        private Dictionary<BuildingTypeId, SpeachBubleHomelessOrderConfig> _homelessSpeachBubleConfigs;
        private Dictionary<BuildingTypeId, BuildingStaticData> _buildings;
        private Dictionary<BuildingTypeId, SchemeStaticData> _schemes;
        private Dictionary<ResourceId, ResourceData> _resources;
        private Dictionary<WorkshopItemId, WorkshopStaticData> _workshops;
        private Dictionary<UnitTypeId, UnitStaticData> _units;
        private Dictionary<EnemyTypeId, EnemyStaticData> _enemies;

        private Dictionary<int, WaveLevelStaticData> _waveLevels;
        private Dictionary<int, BonfireLevelData> _bonfireLevels;

        private MatryoshkaStaticData _matryoshkaStaticData;
        private TooltipStaticData _tooltipStaticData;

        private BonfireIconStaticData _bonfireIconStaticData;


        public PlayerStaticData PlayerStaticData { get; private set; }
        public CheatStaticData CheatStaticData { get; private set; }
        public VagabondStaticData VagabondStaticData { get; private set; }
        public DefaultMaterialStaticData DefaultMaterialStaticData { get; private set; }
        public GameStaticData GameStaticData { get; private set; }
        public LightStaticData LightStaticData { get; private set; }
        public TutorialStaticData TutorialStaticData { get; private set; }
        public CoinsStaticData CoinsStaticData { get; private set; }
        public ForestTransitionStaticData ForestTransitionStaticData { get; private set; }
        public EffectsUIStaticData EnemyEffectStaticData { get; private set; }
        public List<ForestStaticData> ForestStaticDatas { get; private set; }


        public void LoadStaticData()
        {
            _buildingCatalogs = Resources.LoadAll<BuildingCatalogStaticData>(AssetsPath.BuildingCatalogsUIPath)
                .ToDictionary(x => x.Types, x => x);

            _buildings = Resources.LoadAll<BuildingStaticData>(AssetsPath.BuildingsDataPath)
                .ToDictionary(x => x.BuildingTypeId, x => x);

            _schemes = Resources.LoadAll<SchemeStaticData>(AssetsPath.SchemesDataPath)
                .ToDictionary(x => x.BuildingTypeId, x => x);

            _resources = Resources.LoadAll<ResourceStaticData>(AssetsPath.ResourcesDataPath)
                .ToDictionary(x => x.ResourceId, x => x.ResourceData);

            _workshops = Resources.LoadAll<WorkshopStaticData>(AssetsPath.WorkshopsDataPath)
                .ToDictionary(x => x.WorkshopItemId, x => x);

            _units = Resources.LoadAll<UnitStaticData>(AssetsPath.UnitsDataPath)
                .ToDictionary(x => x.UnitTypeId, x => x);

            _enemies = Resources.LoadAll<EnemyStaticData>(AssetsPath.EnemiesDataPath)
                .ToDictionary(x => x.EnemyTypeId, x => x);

            _waveLevels = Resources.LoadAll<WaveLevelStaticData>(AssetsPath.WavesDataPath)
                .ToDictionary(x => x.LevelId, x => x);

            _windows = Resources.Load<WindowStaticData>(UIAssetPath.WindowsStaticDataPath).Configs
                .ToDictionary(x => x.WindowId, x => x);

            _playerSpeachBubleConfigs = Resources.Load<SpeachBubleStaticData>(AssetsPath.SpeachBublePath).PlayerConfigs
                .ToDictionary(x => x.SpeachBubleId, x => x);

            _unitsSpeachBubleConfigs = Resources.Load<SpeachBubleStaticData>(AssetsPath.SpeachBublePath).UnitConfigs
                .ToDictionary(x => x.OrderID, x => x);

            _homelessSpeachBubleConfigs = Resources.Load<SpeachBubleStaticData>(AssetsPath.SpeachBublePath)
                .HomelessConfigs
                .ToDictionary(x => x.BuildingTypeId, x => x);

            _bonfireLevels = Resources.Load<BonfireStaticData>(AssetsPath.BonfireDataPath).Levels
                .ToDictionary(x => x.LevelId, x => x);

            _bonfireIconStaticData = Resources.Load<BonfireIconStaticData>(AssetsPath.BonfireDataIconPath);

            _matryoshkaStaticData = Resources.Load<MatryoshkaStaticData>(AssetsPath.MatryoshkaStaticData);
            _tooltipStaticData = Resources.Load<TooltipStaticData>(AssetsPath.TooltipStaticData);


            GameStaticData = Resources.Load<GameStaticData>(AssetsPath.GameDataPath);
            PlayerStaticData = Resources.Load<PlayerStaticData>(AssetsPath.PlayerStaticDataPath);
            LightStaticData = Resources.Load<LightStaticData>(AssetsPath.LightDataPath);
            CoinsStaticData = Resources.Load<CoinsStaticData>(AssetsPath.CoinsDataPath);
            TutorialStaticData = Resources.Load<TutorialStaticData>(AssetsPath.TutorialPath);
            ForestTransitionStaticData = Resources.Load<ForestTransitionStaticData>(AssetsPath.ForestTransitionData);
            EnemyEffectStaticData = Resources.Load<EffectsUIStaticData>(AssetsPath.EffectUIStaticData);
            DefaultMaterialStaticData = Resources.Load<DefaultMaterialStaticData>(AssetsPath.DefaultMaterialStaticData);
            CheatStaticData = Resources.Load<CheatStaticData>(AssetsPath.CheatStaticDataPath);
            VagabondStaticData = Resources.Load<VagabondStaticData>(AssetsPath.VagabondStaticDataPath);

            ForestStaticDatas = Resources.LoadAll<ForestStaticData>(AssetsPath.ForestDataPath).ToList();
        }


        public BuildingCatalogStaticData ForCatalog(BuildingTypeId buildingTypeId)
        {
            foreach (var catalogStaticData in _buildingCatalogs)
            {
                if (catalogStaticData.Key.Contains(buildingTypeId))
                    return catalogStaticData.Value;
            }

            return null;
        }


        public string ForTooltip<TEnum>(TEnum id) where TEnum : Enum =>
            _tooltipStaticData.GetTooltip(id);

        public Material GetTooltipUnlitMaterial() =>
            _tooltipStaticData.SpritesUnlitDefault;

        public ResourceData ForResource(ResourceId resourceId) =>
            _resources.GetValueOrDefault(resourceId);

        public MatryoshkaData ForNextMatryoshka(EnemyTypeId enemyTypeId, MatryoshkaId currentId)
        {
            if (!_matryoshkaStaticData.MatryoshkaConfigsDictionary.TryGetValue(enemyTypeId,
                    out MatryoshkaConfig matryoshkaConfig))
                return null;

            Array allMatryoshkas = Enum.GetValues(typeof(MatryoshkaId));
            int currentIndex = Array.IndexOf(allMatryoshkas, currentId);

            if (currentIndex < allMatryoshkas.Length - 1)
            {
                MatryoshkaId matryoshkaId = (MatryoshkaId)allMatryoshkas.GetValue(currentIndex + 1);
                return matryoshkaConfig.MatryoshkaDatas.FirstOrDefault(x => x.MatryoshkaId == matryoshkaId);
            }

            return null;
        }

        public Sprite ForSchemeIcon(BuildingTypeId buildingTypeId)
        {
            return _bonfireIconStaticData.SchemeIconsData
                .FirstOrDefault(x => x.BuildingTypeId == buildingTypeId)?.Icon;
        }

        public Sprite ForMainFlagIcon(int level) =>
            _bonfireIconStaticData.MainFlagIconsDatas.FirstOrDefault(x => x.Level == level)?.Icon;

        public RequiredBuildIconData ForRequiredBuilding(RequiredBuildData requiredBuildData)
        {
            return _bonfireIconStaticData.RequiredBuildIconsData.FirstOrDefault(x =>
                x.BuildingTypeId == requiredBuildData.BuildingTypeId && x.LevelId == requiredBuildData.LevelId &&
                x.CardKey == requiredBuildData.CardKey);
        }


        public CoinsIconData ForCoinsIcon() =>
            _bonfireIconStaticData.CoinsIconData;


        public BuildingUpgradeData ForBuilding(
            BuildingTypeId buildingTypeId, BuildingLevelId levelId)
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

        public BuildingUpgradeData ForBuilding(
            BuildingTypeId buildingTypeId,
            BuildingLevelId levelId,
            CardId cardKey)
        {
            if (_buildings.TryGetValue(buildingTypeId, out BuildingStaticData buildingData))
            {
                foreach (BuildingUpgradeData data in buildingData.BuildingsData)
                {
                    if (data.LevelId == levelId && cardKey == data.CardKey)
                        return data;
                }
            }

            return null;
        }


        public BuildingStaticData ForBuilding(BuildingTypeId buildingTypeId) =>
            _buildings.GetValueOrDefault(buildingTypeId);


        public SchemeStaticData ForScheme(BuildingTypeId buildingTypeId) =>
            _schemes.GetValueOrDefault(buildingTypeId);

        public WindowConfig ForWindow(WindowId windowId) =>
            _windows.GetValueOrDefault(windowId);

        public SpeachBubleConfig ForSpeachBuble(SpeachBubleId speachBubleId) =>
            _playerSpeachBubleConfigs.GetValueOrDefault(speachBubleId);

        public SpeachBubleOrderConfig ForSpeachBuble(OrderID orderID) =>
            _unitsSpeachBubleConfigs.GetValueOrDefault(orderID);

        public SpeachBubleHomelessOrderConfig ForSpeachBuble(BuildingTypeId orderID) =>
            _homelessSpeachBubleConfigs.GetValueOrDefault(orderID);

        public UnitStaticData ForUnit(UnitTypeId unitTypeId) =>
            _units.GetValueOrDefault(unitTypeId);

        public EnemyStaticData ForEnemy(EnemyTypeId enemyTypeId) =>
            _enemies.GetValueOrDefault(enemyTypeId);

        public WaveStaticData ForWave(int levelId, int wiveId)
        {
            WaveLevelStaticData levelStaticData = _waveLevels.GetValueOrDefault(levelId);

            return levelStaticData != null ? levelStaticData.Waves.FirstOrDefault(x => x.WaveId == wiveId) : null;
        }

        public BonfireLevelData ForUpgradeBonfire(int levelId) =>
            _bonfireLevels.GetValueOrDefault(levelId);

        public WorkshopStaticData ForWorkshop(WorkshopItemId workshopItemId) =>
            _workshops.GetValueOrDefault(workshopItemId);

        public int GetWavesCount(int levelId)
        {
            WaveLevelStaticData levelStaticData = _waveLevels.GetValueOrDefault(levelId);
            return levelStaticData != null ? levelStaticData.Waves.Count : -1;
        }
    }
}