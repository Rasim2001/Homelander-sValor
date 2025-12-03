using System;
using System.Collections.Generic;
using BuildProcessManagement.WorkshopBuilding;
using Enemy;
using Infastructure.StaticData.Bonfire;
using Infastructure.StaticData.Building;
using Infastructure.StaticData.BuildingCatalog;
using Infastructure.StaticData.CardsData;
using Infastructure.StaticData.Cheats;
using Infastructure.StaticData.Coins;
using Infastructure.StaticData.DefaultMaterial;
using Infastructure.StaticData.EffectsUI;
using Infastructure.StaticData.Enemy;
using Infastructure.StaticData.Forest;
using Infastructure.StaticData.Lights;
using Infastructure.StaticData.Matryoshka;
using Infastructure.StaticData.Player;
using Infastructure.StaticData.RecourceElements;
using Infastructure.StaticData.Schemes;
using Infastructure.StaticData.SpeachBuble.Player;
using Infastructure.StaticData.SpeachBuble.Units;
using Infastructure.StaticData.Tutorial;
using Infastructure.StaticData.Unit;
using Infastructure.StaticData.VagabondCampManagement;
using Infastructure.StaticData.WaveOfEnemies;
using Infastructure.StaticData.Windows;
using Infastructure.StaticData.Workshop;
using Player.Orders;
using UnityEngine;

namespace Infastructure.StaticData.StaticDataService
{
    public interface IStaticDataService
    {
        void LoadStaticData();
        GameStaticData GameStaticData { get; }
        CoinsStaticData CoinsStaticData { get; }
        TutorialStaticData TutorialStaticData { get; }
        List<ForestStaticData> ForestStaticDatas { get; }
        LightStaticData LightStaticData { get; }
        ForestTransitionStaticData ForestTransitionStaticData { get; }
        EffectsUIStaticData EnemyEffectStaticData { get; }
        DefaultMaterialStaticData DefaultMaterialStaticData { get; }
        PlayerStaticData PlayerStaticData { get; }
        CheatStaticData CheatStaticData { get; }
        VagabondStaticData VagabondStaticData { get; }
        UnitStaticData ForUnit(UnitTypeId unitTypeId);
        EnemyStaticData ForEnemy(EnemyTypeId enemyTypeId);
        WaveStaticData ForWave(int levelId, int wiveId);
        int GetWavesCount(int levelId);
        WindowConfig ForWindow(WindowId windowId);
        SpeachBubleConfig ForSpeachBuble(SpeachBubleId speachBubleId);
        BonfireLevelData ForUpgradeBonfire(int levelId);
        WorkshopStaticData ForWorkshop(WorkshopItemId workshopItemId);
        SpeachBubleOrderConfig ForSpeachBuble(OrderID orderID);
        SpeachBubleHomelessOrderConfig ForSpeachBuble(BuildingTypeId orderID);
        ResourceData ForResource(ResourceId resourceId);
        SchemeStaticData ForScheme(BuildingTypeId buildingTypeId);
        BuildingStaticData ForBuilding(BuildingTypeId buildingTypeId);

        BuildingCatalogStaticData ForCatalog(BuildingTypeId buildingTypeId);

        BuildingUpgradeData ForBuilding(
            BuildingTypeId buildingTypeId,
            BuildingLevelId levelId,
            CardId cardKey);

        BuildingUpgradeData ForBuilding(
            BuildingTypeId buildingTypeId,
            BuildingLevelId levelId);

        MatryoshkaData ForNextMatryoshka(EnemyTypeId enemyTypeId, MatryoshkaId currentId);
        string ForTooltip<TEnum>(TEnum id) where TEnum : Enum;
        Material GetTooltipUnlitMaterial();
        Sprite ForSchemeIcon(BuildingTypeId buildingTypeId);
        RequiredBuildIconData ForRequiredBuilding(RequiredBuildData requiredBuildData);
        CoinsIconData ForCoinsIcon();
        Sprite ForMainFlagIcon(int level);
    }
}