using System;
using Player;
using UI.GameplayUI;

namespace Infastructure.Services.BuildModeServices
{
    public class BuildingModifyService : IBuildingModifyService
    {
        public bool IsActive { get; private set; }

        public event Action OnActiveChanged;

        private ShowPriceZone _showPriceZone;
        private ObserverTrigger _buildingObserverTrigger;

        public void Initialize(ShowPriceZone showPriceZone, ObserverTrigger observerTrigger)
        {
            _showPriceZone = showPriceZone;
            _buildingObserverTrigger = observerTrigger;
        }

        public void StartModify()
        {
            IsActive = true;
            OnActiveChanged?.Invoke();

            _showPriceZone.ShowCoins();

            if (_buildingObserverTrigger.CurrentCollider != null &&
                _buildingObserverTrigger.CurrentCollider.TryGetComponent(out HintsDisplayBase HintsDisplayBase))
                HintsDisplayBase.ShowHints();
        }

        public void ExitModify()
        {
            IsActive = false;
            OnActiveChanged?.Invoke();

            if (_buildingObserverTrigger.CurrentCollider != null &&
                _buildingObserverTrigger.CurrentCollider.TryGetComponent(out HintsDisplayBase HintsDisplayBase))
                HintsDisplayBase.HideHints();
        }
    }
}