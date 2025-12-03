using DayCycle;
using Infastructure.StaticData.DayCycle;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(DayCyclePresetStaticData))]
    public class DayCycleEditor : OdinEditor
    {
        private float _sliderValue;
        private DayCycleUpdater _cycleUpdater;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            _sliderValue = EditorGUILayout.Slider("Test Time", _sliderValue, 0.0f, 1.0f);

            if (_cycleUpdater == null)
                _cycleUpdater = FindObjectOfType<DayCycleUpdater>();

            if (_cycleUpdater != null)
                UpdateSlider(_sliderValue);
        }

        private void UpdateSlider(float newValue)
        {
            DayCyclePresetStaticData preset = target as DayCyclePresetStaticData;
            if (preset == null)
                return;

            preset.UpdateScene();
            _sliderValue = newValue;

            float sunriseInPercent = preset.DayNightMaterialData.SunriseInPercent;
            float sunsetInPercent = preset.DayNightMaterialData.SunsetInPercent;
            float dayInPercent = preset.DayNightMaterialData.DayInPercent;

            if (_cycleUpdater == null)
                return;

            _cycleUpdater.DayCycleRotationEditor(newValue);
            _cycleUpdater.UpdateGlobalLightsEditor(newValue, 1);

            if (newValue < 0.5f)
                _cycleUpdater.UpdateDayLightsEditor(newValue, 0.5f);
            else
                _cycleUpdater.UpdateNightLightsEditor(newValue - 0.5f, 0.5f);

            if (newValue < sunriseInPercent / 2)
                _cycleUpdater.UpdateSunriseEditor(newValue, sunriseInPercent / 2);
            else if (newValue > sunsetInPercent / 2)
                _cycleUpdater.UpdateSunsetEditor(newValue - sunsetInPercent / 2, 0.5f - sunsetInPercent / 2);
            else if (newValue > sunriseInPercent / 2 && newValue < dayInPercent / 2)
                _cycleUpdater.UpdateDayEditor(newValue - sunriseInPercent / 2, dayInPercent / 2 - sunriseInPercent / 2);

            SceneView.RepaintAll();
        }
    }
}