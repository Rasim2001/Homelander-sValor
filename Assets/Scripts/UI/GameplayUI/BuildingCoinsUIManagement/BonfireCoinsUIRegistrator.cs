using System;
using Infastructure.Services.PurchaseDelay;
using Infastructure.StaticData.Bonfire;
using Infastructure.StaticData.StaticDataService;
using Units;
using UnityEngine;
using Zenject;

namespace UI.GameplayUI.BuildingCoinsUIManagement
{
    public class BonfireCoinsUIRegistrator : MonoBehaviour
    {
        [SerializeField] private BuildingCoinsUI _buildingCoinsUI;

        private readonly int _firstLevelBonfire = 1;
        private IStaticDataService _staticData;

        private UniqueId _uniqueId;

        [Inject]
        public void Construct(IStaticDataService staticData) =>
            _staticData = staticData;

        private void Start() =>
            UpdateCoinsUI(_firstLevelBonfire);


        public void UpdateCoinsUI(int levelBonfire)
        {
            BonfireLevelData bonfireLevelData = _staticData.ForUpgradeBonfire(levelBonfire);

            if (bonfireLevelData != null)
                _buildingCoinsUI.UpdateCoinsUI(bonfireLevelData.CoinsValue);
            else
                _buildingCoinsUI.Hide();
        }
    }
}