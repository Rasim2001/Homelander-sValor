using System;
using Infastructure.StaticData.Bonfire;
using UI.GameplayUI.BuildingCoinsUIManagement;

namespace Bonfire
{
    public interface IUpgradeMainFlag
    {
        void Upgrade(BonfireMarker bonfireMarker, BuildingCoinsUI buildingCoinsUI);
        bool IsEnoughCoins();
        bool HasUpgrade();
        int LevelIndex { get; set; }
        event Action OnUpgradeFailed;
        event Action OnUpgradeHappened;
        bool HasUpgradeRightNow();
        event Action OnUpgradeFinished;
    }
}