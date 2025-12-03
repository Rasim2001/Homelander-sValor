using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Infastructure.StaticData.Lights
{
    [CreateAssetMenu(fileName = "LightData", menuName = "StaticData/LightData")]
    public class LightStaticData : ScriptableObject
    {
        // @formatter:off
        [FoldoutGroup("Sun Normal")]
        [HorizontalGroup("Sun Normal/Row1", Width = 300)] [LabelWidth(165)] public float SunNormalLightIntensity = 1.57f;
        [HorizontalGroup("Sun Normal/Row1"), LabelText(""), GUIColor(1f, 1f, 1f)] public bool SunNormalIsActive;
        
        [HorizontalGroup("Sun Normal/Row2", Width = 300)] [LabelWidth(165)] public float SunNormalBGLightIntensity = 1.57f;
        [HorizontalGroup("Sun Normal/Row2"), LabelText(""), GUIColor(1f, 1f, 1f)] public bool SunNormalBGIsActive;
        
        [HorizontalGroup("Sun Normal/Row3", Width = 300)] [LabelWidth(165)] public float SunNormalMGLightIntensity = 1.57f;
        [HorizontalGroup("Sun Normal/Row3"), LabelText(""), GUIColor(1f, 1f, 1f)] public bool SunNormalMGIsActive;

        [FoldoutGroup("Sun Mask")]
        [HorizontalGroup("Sun Mask/Row1", Width = 300)] [LabelWidth(165)] public float SunMaskLightIntensity = 5f;
        [HorizontalGroup("Sun Mask/Row1"), LabelText(""), GUIColor(1f, 1f, 1f)] public bool SunMaskIsActive;
        
        [HorizontalGroup("Sun Mask/Row2", Width = 300)] [LabelWidth(165)] public float SunMaskBGLightIntensity = 5f;
        [HorizontalGroup("Sun Mask/Row2"), LabelText(""), GUIColor(1f, 1f, 1f)] public bool SunMaskBGIsActive;
        
        [HorizontalGroup("Sun Mask/Row3", Width = 300)] [LabelWidth(165)] public float SunMaskMGLightIntensity = 5f;
        [HorizontalGroup("Sun Mask/Row3"), LabelText(""), GUIColor(1f, 1f, 1f)] public bool SunMaskMGIsActive;
        
        [FoldoutGroup("Moon Normal")]
        [HorizontalGroup("Moon Normal/Row1", Width = 300)] [LabelWidth(165)] public float MoonNormalLightIntensity = 1.57f;
        [HorizontalGroup("Moon Normal/Row1"), LabelText(""), GUIColor(1f, 1f, 1f)] public bool MoonNormalIsActive;
        
        [HorizontalGroup("Moon Normal/Row2", Width = 300)] [LabelWidth(165)] public float MoonNormalBGLightIntensity = 1.57f;
        [HorizontalGroup("Moon Normal/Row2"), LabelText(""), GUIColor(1f, 1f, 1f)] public bool MoonNormalBGIsActive;
        
        [HorizontalGroup("Moon Normal/Row3", Width = 300)] [LabelWidth(165)] public float MoonNormalMGLightIntensity = 1.57f;
        [HorizontalGroup("Moon Normal/Row3"), LabelText(""), GUIColor(1f, 1f, 1f)] public bool MoonNormalMGIsActive;
        
        [FoldoutGroup("Moon Mask")]
        [HorizontalGroup("Moon Mask/Row1", Width = 300)] [LabelWidth(165)] public float MoonMaskLightIntensity = 13;
        [HorizontalGroup("Moon Mask/Row1"), LabelText(""), GUIColor(1f, 1f, 1f)] public bool MoonMaskIsActive;
        
        [HorizontalGroup("Moon Mask/Row2", Width = 300)] [LabelWidth(165)] public float MoonMaskBGLightIntensity = 13;
        [HorizontalGroup("Moon Mask/Row2"), LabelText(""), GUIColor(1f, 1f, 1f)] public bool MoonMaskBGIsActive;
        
        [HorizontalGroup("Moon Mask/Row3", Width = 300)] [LabelWidth(165)] public float MoonMaskMGLightIntensity = 13;
        [HorizontalGroup("Moon Mask/Row3"), LabelText(""), GUIColor(1f, 1f, 1f)] public bool MoonMaskMGIsActive;
        
        [FoldoutGroup("Global Light")]
        [HorizontalGroup("Global Light/Row1", Width = 300)] [LabelWidth(165)] public float GlobalLightIntensity = 0.25f;
        [HorizontalGroup("Global Light/Row1"), LabelText(""), GUIColor(1f, 1f, 1f)] public bool GlobalLightIsActive;
        
        [HorizontalGroup("Global Light/Row2", Width = 300)] [LabelWidth(165)] public float GlobalBGLightIntensity = 0.25f;
        [HorizontalGroup("Global Light/Row2"), LabelText(""), GUIColor(1f, 1f, 1f)] public bool GlobalBGLightIsActive;
        
        [HorizontalGroup("Global Light/Row3", Width = 300)] [LabelWidth(165)] public float GlobalMGLightIntensity = 0.25f;
        [HorizontalGroup("Global Light/Row3"), LabelText(""), GUIColor(1f, 1f, 1f)] public bool GlobalMGLightIsActive;

        [BoxGroup("Defaults")] [ReadOnly] public float SunNormalLightIntensityReadonly = 1.57f;
        [BoxGroup("Defaults")] [ReadOnly] public float SunMaskLightIntensityReadonly = 5f;
        [BoxGroup("Defaults")] [ReadOnly] public float MoonNormalLightIntensityReadonly = 1.57f;
        [BoxGroup("Defaults")] [ReadOnly] public float MoonMaskLightIntensityReadonly = 13;
        [BoxGroup("Defaults")] [ReadOnly] public float GlobalLightIntensityReadonly = 0.25f;
        
        // @formatter:on

        public event Action OnSettingsChanged;

        [ResponsiveButtonGroup("Settings")]
        public void Apply()
        {
            SunNormalLightIntensityReadonly = SunNormalLightIntensity;
            SunMaskLightIntensityReadonly = SunMaskLightIntensity;

            MoonNormalLightIntensityReadonly = MoonNormalLightIntensity;
            MoonMaskLightIntensityReadonly = MoonMaskLightIntensity;

            GlobalLightIntensityReadonly = GlobalLightIntensity;

            OnSettingsChanged?.Invoke();
        }

        [ResponsiveButtonGroup("Settings")]
        public void Reset()
        {
            SunNormalLightIntensity = SunNormalLightIntensityReadonly;
            SunMaskLightIntensity = SunMaskLightIntensityReadonly;

            MoonNormalLightIntensity = MoonNormalLightIntensityReadonly;
            MoonMaskLightIntensity = MoonMaskLightIntensityReadonly;

            GlobalLightIntensity = GlobalLightIntensityReadonly;

            OnSettingsChanged?.Invoke();
        }
    }
}