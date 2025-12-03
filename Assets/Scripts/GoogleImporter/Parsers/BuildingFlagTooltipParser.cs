using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Infastructure;
using Infastructure.StaticData.Tooltip;
using Infrastructure.StaticData.Tooltip;
using UnityEngine;

namespace GoogleImporter.Parsers
{
    public class BuildingFlagTooltipParser : IGoogleSheetParser
    {
        private readonly TooltipStaticData _toolTipStaticData;

        private BuildingFlagTooltipEntry _buildingFlag;

        public BuildingFlagTooltipParser()
        {
            _toolTipStaticData = Resources.Load<TooltipStaticData>(AssetsPath.TooltipStaticData);
            _toolTipStaticData.BuildingFlagTooltips.Clear();
        }


        public async UniTask Parse(string header, string token)
        {
            switch (header)
            {
                case "ID":
                    _buildingFlag = new BuildingFlagTooltipEntry();
                    _buildingFlag.Id = Enum.Parse<FlagTooltipId>(token);
                    _toolTipStaticData.BuildingFlagTooltips.Add(_buildingFlag);
                    break;
                case "TooltipText":
                    _buildingFlag.Text = token;
                    break;
                default:
                    Debug.Log($"Нет такого Header : {header}");
                    break;
            }
        }
    }
}