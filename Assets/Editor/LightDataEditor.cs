using DayCycle;
using Infastructure.StaticData.Lights;
using Sirenix.OdinInspector.Editor;
using UnityEditor;

namespace Editor
{
    [CustomEditor(typeof(LightStaticData))]
    public class LightDataEditor : OdinEditor
    {
        private DayCycleUpdater _dayCycleUpdater;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            _dayCycleUpdater = FindObjectOfType<DayCycleUpdater>();
            if (_dayCycleUpdater == null)
                return;

            LightStaticData lightStaticData = (LightStaticData)target;

            ApplyLightValues(lightStaticData);
            Toggle(lightStaticData);

            EditorUtility.SetDirty(lightStaticData);
            EditorUtility.SetDirty(_dayCycleUpdater);
        }


        private void ApplyLightValues(LightStaticData lightStaticData)
        {
            _dayCycleUpdater.UpdateLightIntensity(LightTypeId.SunNormal, lightStaticData.SunNormalLightIntensity);
            _dayCycleUpdater.UpdateLightIntensity(LightTypeId.SunBGNormal, lightStaticData.SunNormalBGLightIntensity);
            _dayCycleUpdater.UpdateLightIntensity(LightTypeId.SunMGNormal, lightStaticData.SunNormalMGLightIntensity);

            _dayCycleUpdater.UpdateLightIntensity(LightTypeId.SunMask, lightStaticData.SunMaskLightIntensity);
            _dayCycleUpdater.UpdateLightIntensity(LightTypeId.SunBGMask, lightStaticData.SunMaskBGLightIntensity);
            _dayCycleUpdater.UpdateLightIntensity(LightTypeId.SunMGMask, lightStaticData.SunMaskMGLightIntensity);

            _dayCycleUpdater.UpdateLightIntensity(LightTypeId.MoonNormal, lightStaticData.MoonNormalLightIntensity);
            _dayCycleUpdater.UpdateLightIntensity(LightTypeId.MoonBGNormal, lightStaticData.MoonNormalBGLightIntensity);
            _dayCycleUpdater.UpdateLightIntensity(LightTypeId.MoonMGNormal, lightStaticData.MoonNormalMGLightIntensity);

            _dayCycleUpdater.UpdateLightIntensity(LightTypeId.MoonMask, lightStaticData.MoonMaskLightIntensity);
            _dayCycleUpdater.UpdateLightIntensity(LightTypeId.MoonBGMask, lightStaticData.MoonMaskBGLightIntensity);
            _dayCycleUpdater.UpdateLightIntensity(LightTypeId.MoonMGMask, lightStaticData.MoonMaskMGLightIntensity);

            _dayCycleUpdater.UpdateLightIntensity(LightTypeId.Global, lightStaticData.GlobalLightIntensity);
            _dayCycleUpdater.UpdateLightIntensity(LightTypeId.GlobalBG, lightStaticData.GlobalBGLightIntensity);
            _dayCycleUpdater.UpdateLightIntensity(LightTypeId.GlobalMG, lightStaticData.GlobalMGLightIntensity);
        }


        private void Toggle(LightStaticData lightStaticData)
        {
            _dayCycleUpdater.ToggleLight(LightTypeId.SunNormal, lightStaticData.SunNormalIsActive);
            _dayCycleUpdater.ToggleLight(LightTypeId.SunBGNormal, lightStaticData.SunNormalBGIsActive);
            _dayCycleUpdater.ToggleLight(LightTypeId.SunMGNormal, lightStaticData.SunNormalMGIsActive);

            _dayCycleUpdater.ToggleLight(LightTypeId.SunMask, lightStaticData.SunMaskIsActive);
            _dayCycleUpdater.ToggleLight(LightTypeId.SunBGMask, lightStaticData.SunMaskBGIsActive);
            _dayCycleUpdater.ToggleLight(LightTypeId.SunMGMask, lightStaticData.SunMaskMGIsActive);

            _dayCycleUpdater.ToggleLight(LightTypeId.MoonNormal, lightStaticData.MoonNormalIsActive);
            _dayCycleUpdater.ToggleLight(LightTypeId.MoonBGNormal, lightStaticData.MoonNormalBGIsActive);
            _dayCycleUpdater.ToggleLight(LightTypeId.MoonMGNormal, lightStaticData.MoonNormalMGIsActive);

            _dayCycleUpdater.ToggleLight(LightTypeId.MoonMask, lightStaticData.MoonMaskIsActive);
            _dayCycleUpdater.ToggleLight(LightTypeId.MoonBGMask, lightStaticData.MoonMaskBGIsActive);
            _dayCycleUpdater.ToggleLight(LightTypeId.MoonMGMask, lightStaticData.MoonMaskMGIsActive);

            _dayCycleUpdater.ToggleLight(LightTypeId.Global, lightStaticData.GlobalLightIsActive);
            _dayCycleUpdater.ToggleLight(LightTypeId.GlobalBG, lightStaticData.GlobalBGLightIsActive);
            _dayCycleUpdater.ToggleLight(LightTypeId.GlobalMG, lightStaticData.GlobalMGLightIsActive);
        }
    }
}