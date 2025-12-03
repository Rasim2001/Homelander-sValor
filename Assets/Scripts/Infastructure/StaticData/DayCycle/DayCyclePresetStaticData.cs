using UnityEngine;

namespace Infastructure.StaticData.DayCycle
{
    [CreateAssetMenu(fileName = "DayCyclePresetData", menuName = "StaticData/DayCyclePreset")]
    public class DayCyclePresetStaticData : ScriptableObject
    {
        public DayNightMaterialData DayNightMaterialData;
        public DayNightLightsData DayNightLightsData;

        private DayNightMaterialInizializer _skyUpdater;
        private DayNightLightInitializer _lightsUpdater;


        private void OnValidate() =>
            UpdateScene();

        public void UpdateScene()
        {
            Initialize();
            UpdateDatas();
        }

        private void Initialize()
        {
            if (_skyUpdater == null)
                _skyUpdater = FindObjectOfType<DayNightMaterialInizializer>();
            if (_lightsUpdater == null)
                _lightsUpdater = FindObjectOfType<DayNightLightInitializer>();
        }

        private void UpdateDatas()
        {
            
            if (_skyUpdater != null)
                _skyUpdater.UpdateMaterial(DayNightMaterialData);

            if (_lightsUpdater != null)
                _lightsUpdater.UpdateLight(DayNightLightsData);
        }
    }
}