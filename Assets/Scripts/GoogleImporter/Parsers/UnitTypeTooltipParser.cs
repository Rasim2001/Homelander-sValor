using System;
using Cysharp.Threading.Tasks;
using Infastructure;
using Infastructure.StaticData.Unit;
using Infrastructure.StaticData.Tooltip;
using UnityEngine;

namespace GoogleImporter.Parsers
{
    public class UnitTypeTooltipParser : IGoogleSheetParser
    {
        private readonly TooltipStaticData _toolTipStaticData;

        private UnitTypeTooltipEntry _unitTooltip;

        public UnitTypeTooltipParser()
        {
            _toolTipStaticData = _toolTipStaticData = Resources.Load<TooltipStaticData>(AssetsPath.TooltipStaticData);
            _toolTipStaticData.UnitTypeTooltips.Clear();
        }

      

        public async UniTask Parse(string header, string token)
        {
            switch (header)
            {
                case "ID":
                    _unitTooltip = new UnitTypeTooltipEntry();
                    _unitTooltip.Id = Enum.Parse<UnitTypeId>(token);
                    _toolTipStaticData.UnitTypeTooltips.Add(_unitTooltip);
                    break;
                case "TooltipText":
                    _unitTooltip.Text = token;
                    break;
                default:
                    Debug.Log($"Нет такого Header : {header}");
                    break;
            }
        }
    }
}