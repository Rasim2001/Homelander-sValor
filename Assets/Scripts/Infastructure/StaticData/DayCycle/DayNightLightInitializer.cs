using DayCycle;
using UnityEngine;

namespace Infastructure.StaticData.DayCycle
{
    public class DayNightLightInitializer : MonoBehaviour
    {
        [SerializeField] private DayCycleUpdater _dayCycleUpdater;

        public void UpdateLight(DayNightLightsData lightsData)
        {
            _dayCycleUpdater.UpdateDayCycleLights(
                lightsData.MoonNormalGradient,
                lightsData.MoonMaskGradient,
                lightsData.MoonNormalBGGradient,
                lightsData.MoonMaskBGGradient,
                lightsData.MoonNormalMGGradient,
                lightsData.MoonMaskMGGradient,
                lightsData.SunNormalGradient,
                lightsData.SunMaskGradient,
                lightsData.SunNormalBGGradient,
                lightsData.SunMaskBGGradient,
                lightsData.SunNormalMGGradient,
                lightsData.SunMaskMGGradient,
                lightsData.SunGlareGradient,
                lightsData.GlobalLightGradient,
                lightsData.GlobalLightBGGradient,
                lightsData.GlobalLightMGGradient
            );
        }
    }
}