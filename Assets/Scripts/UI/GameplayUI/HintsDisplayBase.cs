using Infastructure.Services.BuildModeServices;
using Infastructure.Services.Tutorial;
using Player.Orders;
using UnityEngine;
using Zenject;

namespace UI.GameplayUI
{
    public abstract class HintsDisplayBase : MonoBehaviour
    {
        protected ITutorialCheckerService tutorialCheckerService;
        protected IBuildingModeService buildingModeService;

        protected OrderMarker _orderMarker;


        [Inject]
        public void Construct(ITutorialCheckerService tutorialChecker, IBuildingModeService buildingMode)
        {
            tutorialCheckerService = tutorialChecker;
            buildingModeService = buildingMode;
        }


        protected virtual void Awake() =>
            _orderMarker = GetComponent<OrderMarker>();

        public virtual void ShowHints()
        {
            if (_orderMarker == null || buildingModeService.IsBuildingState)
                return;

            Show(true);
        }

        public void HideHints()
        {
            if (buildingModeService.IsBuildingState)
                return;

            Show(false);
        }

        protected abstract void Show(bool value);
    }
}