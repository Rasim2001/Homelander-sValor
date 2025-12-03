using System;
using System.Collections;
using DayCycle;
using Infastructure.Services.AutomatizationService.Builders;
using Infastructure.Services.AutomatizationService.Homeless;
using Infastructure.Services.PlayerProgressService;
using Infastructure.Services.SafeBuildZoneTracker;
using Infastructure.Services.SaveLoadService;
using Infastructure.Services.StreetLight;
using Infastructure.Services.Tutorial;
using Infastructure.Services.UnitEvacuationService;
using Infastructure.Services.Window.GameWindowService;
using Infastructure.StaticData.DayCycle;
using Infastructure.StaticData.StaticDataService;
using Infastructure.StaticData.WaveOfEnemies;
using Infastructure.StaticData.Windows;
using UI.GameplayUI;
using UnityEngine;

namespace Infastructure.Services.EnemyWaves
{
    public class EnemyWavesService : IEnemyWavesService, IDisposable
    {
        private readonly IStaticDataService _staticDataService;
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly IEnemySpawnService _enemySpawnService;
        private readonly IWaveEnemiesCountService _waveEnemiesCountService;
        private readonly IPersistentProgressService _progressService;
        private readonly ISaveLoadService _saveLoadService;
        private readonly IGameWindowService _gameWindowService;

        private WavesProgressBar _wavesProgressBar;
        private Coroutine _coroutine;
        private DayCycleUpdater _dayCycleUpdater;
        private IEnemyWavesService _enemyWavesServiceImplementation;
        private readonly ITutorialCheckerService _tutorialCheckerService;
        private readonly ISafeBuildZone _saveBuildZone;
        private readonly IEvacuationService _evacuationService;
        private readonly IFutureOrdersService _futureOrdersService;
        private readonly IHomelessOrdersService _homelessOrdersService;
        private readonly IStreetLightsService _streetLightsService;


        private bool _isTimeFreezed;

        private int timeWaitOfDay;
        private int timeWaitOfNight;


        public EnemyWavesService(
            IStaticDataService staticDataService,
            ICoroutineRunner coroutineRunner,
            IEnemySpawnService enemySpawnService,
            IWaveEnemiesCountService waveEnemiesCountService,
            IPersistentProgressService progressService,
            ISaveLoadService saveLoadService,
            IGameWindowService gameWindowService,
            ITutorialCheckerService tutorialCheckerService,
            ISafeBuildZone saveBuildZone,
            IEvacuationService evacuationService,
            IFutureOrdersService futureOrdersService,
            IHomelessOrdersService homelessOrdersService,
            IStreetLightsService streetLightsService)
        {
            _staticDataService = staticDataService;
            _coroutineRunner = coroutineRunner;
            _enemySpawnService = enemySpawnService;
            _waveEnemiesCountService = waveEnemiesCountService;
            _progressService = progressService;
            _saveLoadService = saveLoadService;
            _gameWindowService = gameWindowService;
            _tutorialCheckerService = tutorialCheckerService;
            _saveBuildZone = saveBuildZone;
            _evacuationService = evacuationService;
            _futureOrdersService = futureOrdersService;
            _homelessOrdersService = homelessOrdersService;
            _streetLightsService = streetLightsService;
        }


        public void Dispose()
        {
        }

        public void Initialize(WavesProgressBar wavesProgressBar, DayCycleUpdater dayCycleUpdater)
        {
            _wavesProgressBar = wavesProgressBar;
            _dayCycleUpdater = dayCycleUpdater;
        }

        public void StartWaveCycle() =>
            _coroutine = _coroutineRunner.StartCoroutine(StartWaveCycleCoroutine());


        public void FreezTimeEditor() =>
            _isTimeFreezed = true;

        public void UnFreezTimeEditor() =>
            _isTimeFreezed = false;

        public void ForceNightEditor()
        {
            timeWaitOfDay = 0;

            _dayCycleUpdater.ForceNight();
        }

        public void ForceDayEditor()
        {
            timeWaitOfNight = 0;

            _dayCycleUpdater.ForceDay();
        }


        private IEnumerator StartWaveCycleCoroutine()
        {
            _wavesProgressBar.InitializeWaves(GetAllWavesSeconds());

            int levelWaveId = _staticDataService.CheatStaticData.LevelWaveId;
            int savedWaveId = 0;
            int timeAllWaves = 0;

            for (int i = savedWaveId; i < _staticDataService.GetWavesCount(levelWaveId); i++)
            {
                WaveStaticData waveStaticData = _staticDataService.ForWave(levelWaveId, i);

                timeWaitOfDay = waveStaticData.TimeWaitOfDay;
                timeWaitOfNight = waveStaticData.TimeWaitOfNight;

                InitDayCycle(waveStaticData);
                _saveBuildZone.IsNight = false;

                _streetLightsService.HideStreetLight();
                _dayCycleUpdater.SwitchOffStarrySky();
                _futureOrdersService.ClearNightOrders();
                _futureOrdersService.ContinueExecuteOrders();
                _evacuationService.ReleaseOfDefenseUnits();
                _homelessOrdersService.ContinueExecuteOrders();

                while (timeWaitOfDay > 0)
                {
                    _wavesProgressBar.UpdateWavesBar(timeAllWaves);
                    timeWaitOfDay--;
                    timeAllWaves++;

                    if (timeWaitOfDay == waveStaticData.PreNightPreparationTimeInSeconds)
                    {
                        _streetLightsService.ShowStreetLight();
                        _evacuationService.EvacuateAllUnits();
                    }

                    yield return new WaitUntil(() => !_isTimeFreezed);
                    yield return new WaitForSeconds(1);
                }


                _saveBuildZone.IsNight = true;

                _enemySpawnService.StartSpawnEnemies(levelWaveId, i);
                _dayCycleUpdater.SwitchOnStarrySky();
                _futureOrdersService.FilterNightOrders();

                while (timeWaitOfNight > 0)
                {
                    _wavesProgressBar.UpdateWavesBar(timeAllWaves);
                    timeWaitOfNight--;
                    timeAllWaves++;

                    yield return new WaitUntil(() => !_isTimeFreezed);
                    yield return new WaitForSeconds(1);
                }

                yield return new WaitUntil(() => _waveEnemiesCountService.NumberOfEnemiesOnWave == 0);
                ResetCycleDay();
                Save(i);
            }

            _gameWindowService.Open(WindowId.WinWindow);
            _wavesProgressBar.UpdateWavesBar(timeAllWaves);

            if (_coroutine != null)
                _coroutineRunner.StopCoroutine(_coroutine);
        }

        private int GetAllWavesSeconds()
        {
            int levelWaveId = _staticDataService.CheatStaticData.LevelWaveId;

            int total = 0;
            for (int i = 0; i < _staticDataService.GetWavesCount(levelWaveId); i++)
            {
                WaveStaticData waveStaticData = _staticDataService.ForWave(levelWaveId, i);
                total += waveStaticData.TimeWaitOfDay;
                total += waveStaticData.TimeWaitOfNight;
            }

            return total;
        }

        private void Save(int i)
        {
            _progressService.PlayerProgress.DayCycleData.WaveId = i + 1;
            _saveLoadService.SaveProgress();
        }

        private void ResetCycleDay() =>
            _dayCycleUpdater.Reset();

        private void InitDayCycle(WaveStaticData waveStaticData, float tutorialMinutes = 0)
        {
            DayNightLightsData lightsData = waveStaticData.DayCyclePreset.DayNightLightsData;

            _dayCycleUpdater.Initialize(
                waveStaticData.TimeWaitOfDay + tutorialMinutes,
                waveStaticData.TimeWaitOfNight,
                waveStaticData.DayCyclePreset.DayNightMaterialData.SunriseInPercent,
                waveStaticData.DayCyclePreset.DayNightMaterialData.SunsetInPercent,
                waveStaticData.DayCyclePreset.DayNightMaterialData.DayInPercent
            );

            _dayCycleUpdater.UpdateDayCycleLights(
                lightsData.MoonNormalGradient,
                lightsData.MoonMaskGradient,
                lightsData.MoonNormalBGGradient,
                lightsData.MoonMaskBGGradient,
                lightsData.MoonNormalMGGradient,
                lightsData.MoonMaskMGGradient,
                lightsData.SunNormalGradient,
                lightsData.SunMaskGradient,
                lightsData.SunNormalBGGradient,
                lightsData.SunMaskBGGradient,
                lightsData.SunNormalMGGradient,
                lightsData.SunMaskMGGradient,
                lightsData.SunGlareGradient,
                lightsData.GlobalLightGradient,
                lightsData.GlobalLightBGGradient,
                lightsData.GlobalLightMGGradient
            );

            _dayCycleUpdater.UpdateCycleDotween();
        }
    }
}