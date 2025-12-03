using Infastructure.Services.InputPlayerService;
using Infastructure.Services.PauseService;
using UnityEngine;
using Zenject;

namespace UI.GameplayUI.TowerSelectionUI
{
    public abstract class SelectionUIBase : MonoBehaviour
    {
        protected IOrderSelectionUIService orderSelectionUIService;
        protected IPauseService _pauseService;

        [Inject]
        public void Construct(IOrderSelectionUIService orderSelectionService, IPauseService pauseService)
        {
            orderSelectionUIService = orderSelectionService;
            _pauseService = pauseService;
        }
    }
}