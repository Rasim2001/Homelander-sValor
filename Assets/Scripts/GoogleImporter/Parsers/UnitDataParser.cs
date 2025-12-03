using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Infastructure;
using Infastructure.Data;
using Infastructure.StaticData.Unit;
using ModestTree;
using UnityEngine;

namespace GoogleImporter.Parsers
{
    public class UnitDataParser : IGoogleSheetParser
    {
        private readonly Dictionary<UnitTypeId, UnitStaticData> _units;

        private UnitStaticData _unitStaticData;

        public UnitDataParser()
        {
            _units = Resources.LoadAll<UnitStaticData>(AssetsPath.UnitsDataPath)
                .ToDictionary(x => x.UnitTypeId, x => x);
        }

     

        public async UniTask Parse(string header, string token)
        {
            switch (header)
            {
                case "ID":
                    UnitTypeId unitType = Enum.Parse<UnitTypeId>(token);
                    _unitStaticData = ForUnit(unitType);
                    break;
                case "HP":
                    if (!token.IsEmpty())
                        _unitStaticData.Hp = token.ToInt();
                    break;
                case "Damage":
                    if (!token.IsEmpty())
                        _unitStaticData.Damage = token.ToInt();
                    break;
                case "MinimalDistanceMove":
                    if (!token.IsEmpty())
                        _unitStaticData.MinimalDistanceMove = token.ToFloat();
                    break;
                case "WalkSpeed":
                    if (!token.IsEmpty())
                        _unitStaticData.WalkSpeed = token.ToFloat();
                    break;
                case "RunSpeed":
                    if (!token.IsEmpty())
                        _unitStaticData.RunSpeed = token.ToFloat();
                    break;
                case "RunTowardPlayerSpeed":
                    if (!token.IsEmpty())
                        _unitStaticData.RunTowardPlayerSpeed = token.ToFloat();
                    break;
                case "FastRunTowardPlayerSpeed":
                    if (!token.IsEmpty())
                        _unitStaticData.FastRunTowardPlayerSpeed = token.ToFloat();
                    break;
                case "RetreatSpeed":
                    if (!token.IsEmpty())
                        _unitStaticData.RetreatSpeed = token.ToFloat();
                    break;
                case "MinAdditionalFollowSpeed":
                    if (!token.IsEmpty())
                        _unitStaticData.MinAdditionalFollowSpeed = token.ToFloat();
                    break;
                case "MaxAdditionalFollowSpeed":
                    if (!token.IsEmpty())
                        _unitStaticData.MaxAdditionalFollowSpeed = token.ToFloat();
                    break;
                case "MinAnimationSpeed":
                    if (!token.IsEmpty())
                        _unitStaticData.MinAnimationSpeed = token.ToFloat();
                    break;
                case "MaxAnimationSpeed":
                    if (!token.IsEmpty())
                        _unitStaticData.MaxAnimationSpeed = token.ToFloat();
                    break;
                default:
                    Debug.Log($"Нет такого Header : {header}");
                    break;
            }
        }

        private UnitStaticData ForUnit(UnitTypeId unitTypeId) =>
            _units.GetValueOrDefault(unitTypeId);
    }
}