using System;
using System.Collections.Generic;
using System.Linq;
using Infastructure.StaticData.Tooltip;
using Infastructure.StaticData.Unit;
using UnityEngine;

namespace Infrastructure.StaticData.Tooltip
{
    [CreateAssetMenu(fileName = "TooltipStaticData", menuName = "StaticData/TooltipData")]
    public class TooltipStaticData : ScriptableObject
    {
        public List<BuildingFlagTooltipEntry> BuildingFlagTooltips;
        public List<UnitTypeTooltipEntry> UnitTypeTooltips;

        public Material SpritesUnlitDefault;

        private readonly Dictionary<Type, IReadOnlyDictionary<Enum, string>> _tooltipCache = new();

        public string GetTooltip<TEnum>(TEnum id) where TEnum : Enum
        {
            if (!_tooltipCache.ContainsKey(typeof(TEnum)))
                CacheTooltips<TEnum>();

            return _tooltipCache[typeof(TEnum)].TryGetValue(id, out string text) ? text : string.Empty;
        }

        public void Clear()
        {
            BuildingFlagTooltips.Clear();
            UnitTypeTooltips.Clear();
        }


        private void CacheTooltips<TEnum>() where TEnum : Enum
        {
            Dictionary<Enum, string> tooltipDict = new Dictionary<Enum, string>();

            if (typeof(TEnum) == typeof(FlagTooltipId))
            {
                foreach (BuildingFlagTooltipEntry entry in BuildingFlagTooltips)
                {
                    tooltipDict[entry.Id] = entry.Text;
                }
            }
            else if (typeof(TEnum) == typeof(UnitTypeId))
            {
                foreach (UnitTypeTooltipEntry entry in UnitTypeTooltips)
                {
                    tooltipDict[entry.Id] = entry.Text;
                }
            }

            _tooltipCache[typeof(TEnum)] = tooltipDict;
        }
    }

    [Serializable]
    public class BuildingFlagTooltipEntry : TooltipEntry<FlagTooltipId>
    {
    }

    [Serializable]
    public class UnitTypeTooltipEntry : TooltipEntry<UnitTypeId>
    {
    }

    [Serializable]
    public abstract class TooltipEntry<TEnum> where TEnum : Enum
    {
        public TEnum Id;
        public string Text;
    }
}