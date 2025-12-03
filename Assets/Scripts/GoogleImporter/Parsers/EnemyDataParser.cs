using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Enemy;
using Infastructure;
using Infastructure.Data;
using Infastructure.StaticData.Enemy;
using UnityEngine;

namespace GoogleImporter.Parsers
{
    public class EnemyDataParser : IGoogleSheetParser
    {
        private readonly Dictionary<EnemyTypeId, EnemyStaticData> _enemies;
        private EnemyStaticData _currentStaticData;

        public EnemyDataParser()
        {
            _enemies = Resources.LoadAll<EnemyStaticData>(AssetsPath.EnemiesDataPath)
                .ToDictionary(x => x.EnemyTypeId, x => x);
        }

        public async UniTask Parse(string header, string token)
        {
            switch (header)
            {
                case "ID":
                    EnemyTypeId enemyTypeId = Enum.Parse<EnemyTypeId>(token);
                    _currentStaticData = ForEnemy(enemyTypeId);
                    break;
                case "HP":
                    _currentStaticData.Hp = token.ToInt();
                    break;
                case "Speed":
                    _currentStaticData.Speed = token.ToFloat();
                    break;
                case "Damage":
                    _currentStaticData.Damage = token.ToInt();
                    break;
                default:
                    Debug.Log($"Нет такого Header : {header}");
                    break;
            }
        }

        private EnemyStaticData ForEnemy(EnemyTypeId enemyTypeId) =>
            _enemies.GetValueOrDefault(enemyTypeId);
    }
}