using Cysharp.Threading.Tasks;
using Infastructure;
using Infastructure.Data;
using Infastructure.StaticData.VagabondCampManagement;
using UnityEngine;

namespace GoogleImporter.Parsers
{
    public class VagabondCampParser : IGoogleSheetParser
    {
        private VagabondStaticData _vagabondStaticData =
            Resources.Load<VagabondStaticData>(AssetsPath.VagabondStaticDataPath);

        public async UniTask Parse(string header, string token)
        {
            switch (header)
            {
                case "RequiredVagabonds":
                    _vagabondStaticData.RequiredVagabonds = token.ToInt();
                    break;
                case "TimeSpawn":
                    _vagabondStaticData.TimeSpawn = token.ToFloat();
                    break;
                default:
                    Debug.Log($"Нет такого Header : {header}");
                    break;
            }
        }
    }
}