using Sirenix.OdinInspector;
using UnityEngine;

namespace Infastructure.StaticData.Forest
{
    [CreateAssetMenu(fileName = "ForestTransitionData", menuName = "StaticData/ForestTransitionData")]
    public class ForestTransitionStaticData : ScriptableObject
    {
        [Range(0, 1)] public float VignetteValue;
        [Range(1, 20)] public float TransitionSpeed;
        [Range(1, 50)] public float TransitionDistance;

        [FoldoutGroup("AnimationCurves"), ShowIf(nameof(UseLightCurve))]
        public AnimationCurve LightCurve;
        [HideInInspector] public bool UseLightCurve;

        [FoldoutGroup("AnimationCurves"), ShowIf(nameof(UseFogCurve))]
        public AnimationCurve FogCurveUI;
        [HideInInspector] public bool UseFogCurve;

        [FoldoutGroup("AnimationCurves"), ShowIf(nameof(UseVignetteCurve))]
        public AnimationCurve VignetteCurve;
        [HideInInspector] public bool UseVignetteCurve;

        [ResponsiveButtonGroup("AnimationCurves/Curves")]
        [FoldoutGroup("AnimationCurves")]
        [GUIColor("@UseLightCurve ? new Color(0.2f, 1f, 0.2f) :  Color.gray")]
        private void ToggleLightCurve() => UseLightCurve = !UseLightCurve;

        [ResponsiveButtonGroup("AnimationCurves/Curves")]
        [FoldoutGroup("AnimationCurves")]
        [GUIColor("@UseFogCurve ? new Color(0.2f, 1f, 0.2f) : Color.gray")]
        private void ToggleFogCurve() => UseFogCurve = !UseFogCurve;

        [ResponsiveButtonGroup("AnimationCurves/Curves")]
        [FoldoutGroup("AnimationCurves")]
        [GUIColor("@UseVignetteCurve ? new Color(0.2f, 1f, 0.2f) : Color.gray")]
        private void ToggleVignetteCurve() => UseVignetteCurve = !UseVignetteCurve;
    }
}