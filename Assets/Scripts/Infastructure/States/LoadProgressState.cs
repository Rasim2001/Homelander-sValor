using Infastructure.Data;
using Infastructure.Services.PlayerProgressService;
using Infastructure.Services.SaveLoadService;
using Infastructure.StaticData.Player;
using Infastructure.StaticData.StaticDataService;
using UnityEngine;
using Zenject;

namespace Infastructure.States
{
    public class LoadProgressState : IState
    {
        private readonly IStateMachine _stateMachine;
        private readonly IPersistentProgressService _progressService;
        private readonly ISaveLoadService _saveLoadService;
        private readonly IStaticDataService _staticDataService;

        public LoadProgressState(
            IStateMachine stateMachine,
            IPersistentProgressService progressService,
            ISaveLoadService saveLoadService,
            IStaticDataService staticDataService
        )
        {
            _stateMachine = stateMachine;
            _progressService = progressService;
            _saveLoadService = saveLoadService;
            _staticDataService = staticDataService;
        }

        public void Enter()
        {
            LoadProgressOrInitNew();

            _stateMachine.Enter<MainMenuState>();
        }

        public void Exit()
        {
        }

        private void LoadProgressOrInitNew() =>
            _progressService.PlayerProgress = /*_saveLoadService.LoadPlayerProgress() ??*/ NewProgress();

        private PlayerProgress NewProgress()
        {
            Debug.Log("newProgress");
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();

            PlayerStaticData playerStaticData = _staticDataService.PlayerStaticData;

            return new PlayerProgress(playerStaticData.AmountOfCoins);
        }


        public class Factory : PlaceholderFactory<IStateMachine, LoadProgressState>
        {
        }
    }
}