using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Infastructure.StaticData.DayCycle
{
    [Serializable]
    public class DayNightMaterialData
    {
        [FoldoutGroup("Sunrise")] public Color ColorSinriseA;
        [FoldoutGroup("Sunrise")] public float ColorSinriseAValue;

        [FoldoutGroup("Sunrise")] public Color ColorSinriseB;
        [FoldoutGroup("Sunrise")] public float ColorSinriseBValue;

        [FoldoutGroup("Day")] public Color ColorDayA;
        [FoldoutGroup("Day")] public float ColorDayAValue;
        [FoldoutGroup("Day")] public Color ColorDayB;
        [FoldoutGroup("Day")] public float ColorDayBValue;


        [FoldoutGroup("Sunset")] public Color ColorSunsetA;
        [FoldoutGroup("Sunset")] public float ColorSunsetAValue;
        [FoldoutGroup("Sunset")] public Color ColorSunsetB;
        [FoldoutGroup("Sunset")] public float ColorSunsetBValue;

        [FoldoutGroup("Night")] public Color ColorNightA;
        [FoldoutGroup("Night")] public float ColorNightAValue;
        [FoldoutGroup("Night")] public Color ColorNightB;
        [FoldoutGroup("Night")] public float ColorNightBValue;

        [FoldoutGroup("Percentages")][Range(0, 1)] public float SunriseInPercent;
        [FoldoutGroup("Percentages")][Range(0, 1)] public float SunsetInPercent;
        [FoldoutGroup("Percentages")][Range(0, 1)] public float DayInPercent;
    }
}