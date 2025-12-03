using Infastructure.Data;
using Infastructure.Services.PlayerProgressService;
using Infastructure.Services.ProgressWatchers;
using UnityEngine;

namespace Infastructure.Services.SaveLoadService
{
    public class SaveLoadService : ISaveLoadService
    {
        private const string ProgressKey = "Progress";

        private readonly IPersistentProgressService _progressService;
        private readonly IProgressWatchersService _progressWatchersService;

        public SaveLoadService(IPersistentProgressService progressService,
            IProgressWatchersService progressWatchersService)
        {
            _progressService = progressService;
            _progressWatchersService = progressWatchersService;
        }

        public void SaveProgress()
        {
            /*_progressService.PlayerProgress.UnitDataListWrapper.Units.Clear();

            foreach (ISavedProgress progressWriter in _progressWatchersService.ProgressWriters)
                progressWriter.UpdateProgress(_progressService.PlayerProgress);

            Debug.Log("Save Progress");
            PlayerPrefs.SetString(ProgressKey, _progressService.PlayerProgress.ToJson());*/
        }

        public PlayerProgress LoadPlayerProgress()
        {
            return null; //PlayerPrefs.GetString(ProgressKey)?.ToDeserialized<PlayerProgress>();
        }
    }
}