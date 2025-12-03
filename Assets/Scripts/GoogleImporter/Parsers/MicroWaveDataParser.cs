using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Enemy;
using Infastructure.Data;
using Infastructure.StaticData.WaveOfEnemies;
using ModestTree;
using UnityEngine;
using EnemyWaveInfo = Infastructure.StaticData.WaveOfEnemies.EnemyWaveInfo;

namespace GoogleImporter.Parsers
{
    public class MicroWaveDataParser : IGoogleSheetParser
    {
        private readonly WaveStaticData _waveStaticData;

        private EnemyWaveInfo _enemyWaveInfo;

        public MicroWaveDataParser(WaveStaticData waveStaticData) =>
            _waveStaticData = waveStaticData;


        public async UniTask Parse(string header, string token)
        {
            if (token.IsEmpty())
                return;

            switch (header)
            {
                case "EnemyTypeId":
                    _enemyWaveInfo = new EnemyWaveInfo();
                    _waveStaticData.MicroWavesInfo.Waves.Add(_enemyWaveInfo);

                    EnemyTypeId enemyTypeId = Enum.Parse<EnemyTypeId>(token);
                    _enemyWaveInfo.EnemyTypeId = enemyTypeId;
                    break;
                case "Amount":
                    _enemyWaveInfo.Amount = token.ToInt();
                    break;
                case "TimeBetweenSpawnEnemy":
                    _enemyWaveInfo.TimeBetweenSpawnEnemy = token.ToFloat();
                    break;
                default:
                    Debug.Log($"Нет такого Header : {header}");
                    break;
            }
        }
    }
}