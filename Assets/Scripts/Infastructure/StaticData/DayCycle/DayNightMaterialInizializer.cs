using UnityEngine;

namespace Infastructure.StaticData.DayCycle
{
    [ExecuteInEditMode]
    public class DayNightMaterialInizializer : MonoBehaviour
    {
        private const string Color01 = "_Color01";
        private const string Color01Value = "_Color01Value";
        private const string Color02 = "_Color02";
        private const string Color02Value = "_Color02Value";
        private const string ColorSunset = "_ColorSunset";
        private const string ColorSunset2 = "_ColorSunset2";
        private const string ColorSunsetValue = "_ColorSunsetValue";
        private const string ColorSunsetValue2 = "_ColorSunsetValue2";
        private const string ColorNight = "_ColorNight";
        private const string ColorNight2 = "_ColorNight2";
        private const string ColorNightValue = "_ColorNightValue";
        private const string ColorNightValue2 = "_ColorNightValue2";

        private const string ColorDay = "_ColorDay";
        private const string ColorDay2 = "_ColorDay2";
        private const string ColorDayValue = "_ColorDayValue";
        private const string ColorDayValue2 = "_ColorDayValue2";

        private static readonly int Color03 = Shader.PropertyToID(Color01);
        private static readonly int Color04 = Shader.PropertyToID(Color02);
        private static readonly int Value = Shader.PropertyToID(Color02Value);
        private static readonly int Color01Value1 = Shader.PropertyToID(Color01Value);
        private static readonly int Sunset = Shader.PropertyToID(ColorSunset);
        private static readonly int Sunset2 = Shader.PropertyToID(ColorSunset2);
        private static readonly int SunsetValue = Shader.PropertyToID(ColorSunsetValue);
        private static readonly int SunsetValue2 = Shader.PropertyToID(ColorSunsetValue2);
        private static readonly int Night = Shader.PropertyToID(ColorNight);
        private static readonly int Night2 = Shader.PropertyToID(ColorNight2);
        private static readonly int NightValue = Shader.PropertyToID(ColorNightValue);
        private static readonly int NightValue2 = Shader.PropertyToID(ColorNightValue2);

        private static readonly int Day = Shader.PropertyToID(ColorDay);
        private static readonly int Day2 = Shader.PropertyToID(ColorDay2);
        private static readonly int DayValue = Shader.PropertyToID(ColorDayValue);
        private static readonly int DayValue2 = Shader.PropertyToID(ColorDayValue2);

        [SerializeField] private Material _material;

        public void UpdateMaterial(DayNightMaterialData dayNightMaterialData)
        {
            _material.SetColor(Color03, dayNightMaterialData.ColorSinriseA);
            _material.SetColor(Color04, dayNightMaterialData.ColorSinriseB);
            _material.SetFloat(Value, dayNightMaterialData.ColorSinriseBValue);

            _material.SetFloat(Color01Value1, dayNightMaterialData.ColorSinriseAValue);
            _material.SetColor(Sunset, dayNightMaterialData.ColorSunsetA);
            _material.SetColor(Sunset2, dayNightMaterialData.ColorSunsetB);
            _material.SetFloat(SunsetValue, dayNightMaterialData.ColorSunsetAValue);
            _material.SetFloat(SunsetValue2, dayNightMaterialData.ColorSunsetBValue);

            _material.SetColor(Night, dayNightMaterialData.ColorNightA);
            _material.SetColor(Night2, dayNightMaterialData.ColorNightB);
            _material.SetFloat(NightValue, dayNightMaterialData.ColorNightAValue);
            _material.SetFloat(NightValue2, dayNightMaterialData.ColorNightBValue);

            _material.SetColor(Day, dayNightMaterialData.ColorDayA);
            _material.SetColor(Day2, dayNightMaterialData.ColorDayB);
            _material.SetFloat(DayValue, dayNightMaterialData.ColorDayAValue);
            _material.SetFloat(DayValue2, dayNightMaterialData.ColorDayBValue);
        }
    }
}