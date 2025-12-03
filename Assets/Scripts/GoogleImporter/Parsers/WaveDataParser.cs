using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Infastructure;
using Infastructure.Data;
using Infastructure.StaticData.WaveOfEnemies;
using ModestTree;
using UnityEngine;

namespace GoogleImporter.Parsers
{
    public class WaveDataParser : IGoogleSheetParser
    {
        private readonly IImporter _Importer;

        private readonly Dictionary<int, WaveStaticData> _waves;

        private WaveStaticData _currentWave;

        public WaveDataParser(IImporter Importer, List<WaveStaticData> waves)
        {
            _Importer = Importer;

            _waves = waves.ToDictionary(x => x.WaveId, x => x);
        }


        public async UniTask Parse(string header, string token)
        {
            if (token.IsEmpty())
                return;

            switch (header)
            {
                case "ID":
                    int waveId = token.ToInt();

                    if (_waves.TryGetValue(waveId, out WaveStaticData wave))
                        _currentWave = wave;
                    else
                    {
                        if (_currentWave != null)
                        {
                            Debug.LogWarning($"Wave with ID {waveId}");

                            WaveStaticData waveStaticData = ScriptableObjectUtilityCreator
                                .CreateAsset<WaveStaticData>("Assets/Resources/StaticData/Waves", $"Wave{token}");

                            waveStaticData.WaveId = waveId;
                            waveStaticData.DayCyclePreset.DayNightLightsData =
                                _currentWave.DayCyclePreset.DayNightLightsData;
                            waveStaticData.DayCyclePreset.DayNightMaterialData =
                                _currentWave.DayCyclePreset.DayNightMaterialData;

                            _currentWave = waveStaticData;
                        }
                    }

                    break;
                case "TimeWaitOfDay":
                    _currentWave.TimeWaitOfDay = token.ToInt();
                    break;
                case "TimeWaitOfNight":
                    _currentWave.TimeWaitOfNight = token.ToInt();
                    break;
                case "TimeBetweenMicroWaves":
                    _currentWave.TimeBetweenMicroWaves = token.ToInt();
                    break;
                case "PassedWaveCoins":
                    _currentWave.PassedWaveCoins = token.ToInt();
                    break;
                case "PreNightPreparationTimeInSeconds":
                    _currentWave.PreNightPreparationTimeInSeconds = token.ToInt();
                    break;
                case "MicroWavesInfo":
                    await LoadMicroWaveInfo(token);
                    break;
                default:
                    Debug.Log($"Нет такого Header : {header}");
                    break;
            }
        }

        private async UniTask LoadMicroWaveInfo(string microWaveIdSheet)
        {
            IGoogleSheetParser microWaveParser = new MicroWaveDataParser(_currentWave);

            _currentWave.MicroWavesInfo = new MicroWavesInfo();
            _currentWave.MicroWavesInfo.Waves = new List<EnemyWaveInfo>();

            await _Importer.DownloadAndParseSheet(microWaveIdSheet, microWaveParser);
        }
    }
}