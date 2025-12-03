using Infastructure.Services.ECSInput;
using Infastructure.Services.PauseService;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Windows
{
    public class GameSettingsWindow : WindowBase, IEcsWatcherWindow
    {
        [SerializeField] private YesNoMiniWindow _yesNoMiniWindow;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _backToMainButton;

        private IPauseService _pauseService;
        private IEcsWatchersService _ecsWatchersService;

        [Inject]
        public void Consturct(IPauseService pauseService, IEcsWatchersService ecsWatchersService)
        {
            _ecsWatchersService = ecsWatchersService;
            _pauseService = pauseService;
        }


        protected override void Initialize()
        {
            _pauseService.TurnOn();

            base.Initialize();
        }

        protected override void SubscribeUpdates()
        {
            base.SubscribeUpdates();

            _restartButton.onClick.AddListener(OpenMiniWindowYesNo);
            _backToMainButton.onClick.AddListener(BackToMainMenu);
        }

        protected override void Cleanup()
        {
            _pauseService.TurnOff();

            _restartButton.onClick.RemoveListener(OpenMiniWindowYesNo);
            _backToMainButton.onClick.RemoveListener(BackToMainMenu);

            _ecsWatchersService.Release(this);

            base.Cleanup();
        }

        private void OpenMiniWindowYesNo()
        {
            _yesNoMiniWindow.SetRestart(true);
            _yesNoMiniWindow.Initialize();
        }

        private void BackToMainMenu()
        {
            _yesNoMiniWindow.SetRestart(false);
            _yesNoMiniWindow.Initialize();
        }

        public void EcsCancel() =>
            Destroy(gameObject);
    }
}