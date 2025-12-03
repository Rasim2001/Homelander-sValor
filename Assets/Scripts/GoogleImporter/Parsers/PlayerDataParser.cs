using Cysharp.Threading.Tasks;
using Infastructure;
using Infastructure.Data;
using Infastructure.StaticData.Player;
using UnityEngine;

namespace GoogleImporter.Parsers
{
    public class PlayerDataParser : IGoogleSheetParser
    {
        private readonly PlayerStaticData _playerStaticData =
            Resources.Load<PlayerStaticData>(AssetsPath.PlayerStaticDataPath);

        public async UniTask Parse(string header, string token)
        {
            switch (header)
            {
                case "Speed":
                    _playerStaticData.Speed = token.ToFloat();
                    break;
                case "AccelerationTime":
                    _playerStaticData.AccelerationTime = token.ToFloat();
                    break;
                case "Hp":
                    _playerStaticData.Hp = token.ToInt();
                    break;
                case "BuildModeDelay":
                    _playerStaticData.BuildModeDelay = token.ToFloat();
                    break;
                case "AmountOfCoins":
                    _playerStaticData.AmountOfCoins = token.ToInt();
                    break;
                case "ShootDamage":
                    _playerStaticData.ShootDamage = token.ToInt();
                    break;
                default:
                    Debug.Log($"Нет такого Header : {header}");
                    break;
            }
        }
    }
}