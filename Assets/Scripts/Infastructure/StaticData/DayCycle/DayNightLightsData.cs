using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Infastructure.StaticData.DayCycle
{
    [Serializable]
    public class DayNightLightsData
    {
        [FoldoutGroup("Night Light")] public GradientField MoonNormalGradient;
        [FoldoutGroup("Night Light")] public GradientField MoonMaskGradient;
        [FoldoutGroup("Night Light")] public GradientField MoonNormalBGGradient;
        [FoldoutGroup("Night Light")] public GradientField MoonMaskBGGradient;
        [FoldoutGroup("Night Light")] public GradientField MoonNormalMGGradient;
        [FoldoutGroup("Night Light")] public GradientField MoonMaskMGGradient;

        [FoldoutGroup("Day Light")] public GradientField SunNormalGradient;
        [FoldoutGroup("Day Light")] public GradientField SunMaskGradient;
        [FoldoutGroup("Day Light")] public GradientField SunNormalBGGradient;
        [FoldoutGroup("Day Light")] public GradientField SunMaskBGGradient;
        [FoldoutGroup("Day Light")] public GradientField SunNormalMGGradient;
        [FoldoutGroup("Day Light")] public GradientField SunMaskMGGradient;

        [FoldoutGroup("Global Light")] public GradientField GlobalLightGradient;
        [FoldoutGroup("Global Light")] public GradientField GlobalLightBGGradient;
        [FoldoutGroup("Global Light")] public GradientField GlobalLightMGGradient;

        [FoldoutGroup("Sun glare")] public GradientField SunGlareGradient;
    }
}