using Infastructure.Services.BuildModeServices;
using Infastructure.Services.Tutorial;
using Infastructure.Services.Tutorial.TutorialProgress;
using Player.Orders;
using UnityEngine;
using Zenject;

namespace UI.GameplayUI
{
    public abstract class HintsDisplayBase : MonoBehaviour
    {
        protected ITutorialProgressService TutorialProgressService;
        protected IBuildingModeService buildingModeService;

        protected OrderMarker _orderMarker;


        [Inject]
        public void Construct(ITutorialProgressService tutorialProgressService, IBuildingModeService buildingMode)
        {
            TutorialProgressService = tutorialProgressService;
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