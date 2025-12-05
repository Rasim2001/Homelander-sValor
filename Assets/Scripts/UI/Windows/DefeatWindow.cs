using Infastructure.Services.PauseService;
using Infastructure.States;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Windows
{
    public class DefeatWindow : WindowBase
    {
        [SerializeField] private Button _backToMenu;

        private IStateMachine _stateMachine;
        private IPauseService _pauseService;

        [Inject]
        public void Construct(IStateMachine stateMachine, IPauseService pauseService)
        {
            _stateMachine = stateMachine;
            _pauseService = pauseService;
        }

        protected override void Initialize()
        {
            base.Initialize();

            _pauseService.TurnOn();
        }

        protected override void SubscribeUpdates()
        {
            base.SubscribeUpdates();

            _backToMenu.onClick.AddListener(BackToMenu);
        }

        protected override void Cleanup()
        {
            base.Cleanup();

            _pauseService.TurnOff();
            _backToMenu.onClick.RemoveListener(BackToMenu);
        }

        private void BackToMenu() =>
            _stateMachine.Enter<LoadProgressState>();
    }
}