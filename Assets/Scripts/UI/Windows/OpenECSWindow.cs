using Infastructure.Services.PauseService;
using Infastructure.Services.Window.GameWindowService;
using Infastructure.StaticData.Windows;
using UnityEngine;
using Zenject;

namespace UI.Windows
{
    public class OpenECSWindow : MonoBehaviour
    {
        private IGameWindowService _gameWindowService;
        private IPauseService _pauseService;

        [Inject]
        public void Construct(IGameWindowService gameWindowService, IPauseService pauseService)
        {
            _gameWindowService = gameWindowService;
            _pauseService = pauseService;
        }

        public void Update()
        {
            /*if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!_pauseService.IsPaused)
                    _gameWindowService.Open(WindowId.GameSettingsWindow);
                else
                {
                    GameSettingsWindow gameSettingsWindow = gameObject.GetComponentInChildren<GameSettingsWindow>();
                    Destroy(gameSettingsWindow.gameObject);
                }
            }*/
        }
    }
}