using System;
using DG.Tweening;
using Infastructure.StaticData.Lights;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace DayCycle
{
    public class DayCycleUpdater : MonoBehaviour
    {
        [SerializeField] private AnimationCurve _dayCurve;

        private static readonly int TimeFactor = Shader.PropertyToID("_TimeFactor");
        private static readonly int TimeFactorNight = Shader.PropertyToID("_TimeFactorNight");
        private static readonly int TimeFactorDay = Shader.PropertyToID("_TimeFactorDay");

        [Header("Night Light")]
        [SerializeField] private Light2D _moonNormalLight;
        [SerializeField] private Light2D _moonMaskLight;
        [SerializeField] private Light2D _moonNormalBGLight;
        [SerializeField] private Light2D _moonMaskBGLight;
        [SerializeField] private Light2D _moonNormalMGLight;
        [SerializeField] private Light2D _moonMaskMGLight;

        [Header("Day Light")]
        [SerializeField] private Light2D _sunNormalLight;
        [SerializeField] private Light2D _sunMaskLight;
        [SerializeField] private Light2D _sunNormalBGLight;
        [SerializeField] private Light2D _sunMaskBGLight;
        [SerializeField] private Light2D _sunNormalMGLight;
        [SerializeField] private Light2D _sunMaskMGLight;

        [Header("Global Light")]
        [SerializeField] private Light2D _globalLight;
        [SerializeField] private Light2D _globalBGLight;
        [SerializeField] private Light2D _globalMGLight;

        [Header("Other Lights")]
        [SerializeField] private GameObject[] _streetLights;

        [Header("Other Settings")]
        [SerializeField] private GameObject _skyParticleSystem;
        [SerializeField] private Transform _dayCycleTransform;
        [SerializeField] private SpriteRenderer _sunSpriteRender;
        [SerializeField] private Material _material;

        public float CurrentDayTime { get; private set; }
        public float CurrentNightime { get; private set; }

        private GradientField _moonNormalGradient;
        private GradientField _moonMaskGradient;

        private GradientField _moonNormalBGGradient;
        private GradientField _moonMaskBGGradient;

        private GradientField _moonNormalMGGradient;
        private GradientField _moonMaskMGGradient;
        private GradientField _sunNormalGradient;
        private GradientField _sunMaskGradient;

        private GradientField _sunNormalBGGradient;
        private GradientField _sunMaskBGGradient;

        private GradientField _sunNormalMGGradient;
        private GradientField _sunMaskMGGradient;
        private GradientField _sunGlareGradient;
        private GradientField _globalLightGradient;
        private GradientField _globalLightBGGradient;
        private GradientField _globalLightMGGradient;

        private bool _isForcingNight;
        private bool _isForcingDay;

        private float _dayTime;
        private float _nightTime;
        private float _sunriseInPercent;
        private float _sunsetInPercent;
        private float _dayInPercent;
        private Sequence _sequence;

        private Tween _nightTween;
        private Tween _dayTween;
        private Light2D _cristalLight;


        private void Start()
        {
            _material.SetFloat(TimeFactor, 0);
            _material.SetFloat(TimeFactorDay, 0);
            _material.SetFloat(TimeFactorNight, 1);

            transform.localRotation = Quaternion.Euler(0, 0, 90f);
        }

        public void SwitchOnStarrySky() =>
            _skyParticleSystem.SetActive(true);

        public void SwitchOffStarrySky() =>
            _skyParticleSystem.SetActive(false);

        public void Initialize(
            float dayTime,
            float nightTime,
            float sunriseInPercent,
            float sunsetInPercent,
            float dayInPercent
        )
        {
            _dayTime = dayTime;
            _nightTime = nightTime;
            _sunriseInPercent = sunriseInPercent;
            _sunsetInPercent = sunsetInPercent;
            _dayInPercent = dayInPercent;
        }

        public void InitializeCristalLight(Light2D cristalLight) =>
            _cristalLight = cristalLight;

        public void UpdateDayCycleLights(
            GradientField moonNormalGradient,
            GradientField moonMaskGradient,
            GradientField moonNormalBGGradient,
            GradientField moonMaskBGGradient,
            GradientField moonNormalMGGradient,
            GradientField moonMaskMGGradient,
            GradientField sunNormalGradient,
            GradientField sunMaskGradient,
            GradientField sunNormalBGGradient,
            GradientField sunMaskBGGradient,
            GradientField sunNormalMGGradient,
            GradientField sunMaskMGGradient,
            GradientField sunGlareGradient,
            GradientField globalLightGradient,
            GradientField globalLightBGGradient,
            GradientField globalLightMGGradient
        )
        {
            _globalLightBGGradient = globalLightBGGradient;
            _globalLightMGGradient = globalLightMGGradient;
            _globalLightGradient = globalLightGradient;

            _sunNormalMGGradient = sunNormalMGGradient;
            _sunMaskMGGradient = sunMaskMGGradient;

            _moonMaskMGGradient = moonMaskMGGradient;
            _moonNormalMGGradient = moonNormalMGGradient;

            _moonNormalGradient = moonNormalGradient;
            _moonMaskGradient = moonMaskGradient;
            _moonNormalBGGradient = moonNormalBGGradient;
            _moonMaskBGGradient = moonMaskBGGradient;

            _sunNormalGradient = sunNormalGradient;
            _sunMaskGradient = sunMaskGradient;
            _sunNormalBGGradient = sunNormalBGGradient;
            _sunMaskBGGradient = sunMaskBGGradient;

            _sunGlareGradient = sunGlareGradient;
        }

        public void Reset()
        {
            _dayCycleTransform.rotation = Quaternion.Euler(0, 0, 90f);

            _material.SetFloat(TimeFactor, 0);
            _material.SetFloat(TimeFactorDay, 0);
            _material.SetFloat(TimeFactorNight, 1);

            CurrentNightime = 0;
            CurrentDayTime = 0;
        }


        private void OnDestroy() =>
            _sequence.Kill();

        public void UpdateCycleDotween()
        {
            _dayCycleTransform.DOKill();
            _sequence.Kill();
            _nightTween.Kill();
            _dayTween.Kill();

            _sequence = DOTween.Sequence();
            _nightTween = null;
            _dayTween = null;

            _dayTween = _dayCycleTransform
                .DOLocalRotate(new Vector3(0, 0, -90f), _dayTime)
                .SetEase(_dayCurve)
                .SetUpdate(UpdateType.Normal)
                .OnUpdate(() =>
                {
                    if (_dayTween == null)
                        return;

                    float currentDayTime = _dayTween.Elapsed();
                    CurrentDayTime = currentDayTime;

                    UpdateGlobalLights(currentDayTime, _dayTime + _nightTime);
                    UpdateDayLights(currentDayTime, _dayTime);

                    if (currentDayTime < _dayTime * _sunriseInPercent)
                        UpdateSunrise(currentDayTime, _dayTime * _sunriseInPercent);
                    else if (currentDayTime > _dayTime * _sunsetInPercent)
                        UpdateSunset(currentDayTime - _dayTime * _sunsetInPercent,
                            _dayTime - _dayTime * _sunsetInPercent);
                    else if (currentDayTime > _dayTime * _sunriseInPercent && currentDayTime < _dayTime * _dayInPercent)
                    {
                        UpdateDay(currentDayTime - _dayTime * _sunriseInPercent,
                            _dayTime * _dayInPercent - _dayTime * _sunriseInPercent);
                    }
                }).OnComplete(PrepareForNight);

            _nightTween = _dayCycleTransform
                .DOLocalRotate(new Vector3(0, 0, -270f), _nightTime)
                .SetEase(_dayCurve)
                .SetUpdate(UpdateType.Normal)
                .OnUpdate(() =>
                {
                    if (_nightTween == null)
                        return;

                    float currentNightTime = _nightTween.Elapsed();
                    CurrentNightime = currentNightTime;

                    if (currentNightTime < _nightTime * _sunriseInPercent)
                    {
                        if (!_isForcingNight)
                            ChangeSequenceTimeScale(10);
                    }

                    else if (currentNightTime > _nightTime * _sunsetInPercent)
                    {
                        if (!_isForcingDay)
                            ChangeSequenceTimeScale(0);
                    }
                    else if (currentNightTime > _nightTime * _sunriseInPercent &&
                             currentNightTime < _nightTime * _dayInPercent)
                    {
                        if (!_isForcingNight)
                            ChangeSequenceTimeScale(1);
                    }

                    UpdateGlobalLights(_dayTime + currentNightTime, _dayTime + _nightTime);
                    UpdateNightLights(currentNightTime, _nightTime);
                });

            _sequence.Append(_dayTween);
            _sequence.Append(_nightTween);
        }

        private void PrepareForNight()
        {
            ActiveStreetLight(true);
            _sunSpriteRender.color = _sunGlareGradient.Evaluate(0);
        }

        private void ActiveStreetLight(bool value)
        {
            foreach (GameObject streetLight in _streetLights)
                streetLight.SetActive(value);
        }

        private void UpdateGlobalLights(float currentTime, float dayCycleTime)
        {
            _globalLightGradient.RefreshCache();
            _globalLight.color = _globalLightGradient.Evaluate(currentTime / dayCycleTime);

            _globalLightBGGradient.RefreshCache();
            _globalBGLight.color = _globalLightBGGradient.Evaluate(currentTime / dayCycleTime);

            _globalLightMGGradient.RefreshCache();
            _globalMGLight.color = _globalLightMGGradient.Evaluate(currentTime / dayCycleTime);
        }

        private void UpdateDayLights(float currentDayTime, float dayTime)
        {
            _sunGlareGradient.RefreshCache();
            _sunSpriteRender.color = _sunGlareGradient.Evaluate(currentDayTime / dayTime);

            _sunNormalGradient.RefreshCache();
            _sunNormalLight.color = _sunNormalGradient.Evaluate(currentDayTime / dayTime);

            _sunNormalBGGradient.RefreshCache();
            _sunNormalBGLight.color = _sunNormalBGGradient.Evaluate(currentDayTime / dayTime);

            _sunNormalMGGradient.RefreshCache();
            _sunNormalMGLight.color = _sunNormalMGGradient.Evaluate(currentDayTime / dayTime);

            _sunMaskGradient.RefreshCache();
            _sunMaskLight.color = _sunMaskGradient.Evaluate(currentDayTime / dayTime);

            _sunMaskBGGradient.RefreshCache();
            _sunMaskBGLight.color = _sunMaskBGGradient.Evaluate(currentDayTime / dayTime);

            _sunMaskMGGradient.RefreshCache();
            _sunMaskMGLight.color = _sunMaskMGGradient.Evaluate(currentDayTime / dayTime);
        }

        private void UpdateNightLights(float currentNightTime, float nightTime)
        {
            _moonNormalGradient.RefreshCache();
            _moonNormalLight.color = _moonNormalGradient.Evaluate(currentNightTime / nightTime);

            _moonNormalBGGradient.RefreshCache();
            _moonNormalBGLight.color = _moonNormalBGGradient.Evaluate(currentNightTime / nightTime);

            _moonNormalMGGradient.RefreshCache();
            _moonNormalMGLight.color = _moonNormalMGGradient.Evaluate(currentNightTime / nightTime);

            _moonMaskGradient.RefreshCache();
            _moonMaskLight.color = _moonMaskGradient.Evaluate(currentNightTime / nightTime);

            _moonMaskBGGradient.RefreshCache();
            _moonMaskBGLight.color = _moonMaskBGGradient.Evaluate(currentNightTime / nightTime);

            _moonMaskMGGradient.RefreshCache();
            _moonMaskMGLight.color = _moonMaskMGGradient.Evaluate(currentNightTime / nightTime);
        }

        private void UpdateSunrise(float sunriseProgress, float sunriseDuration)
        {
            float timeFactor = Mathf.Lerp(1f, 0f, sunriseProgress / sunriseDuration);
            _material.SetFloat(TimeFactorNight, timeFactor);

            if (!_isForcingNight)
                ChangeSequenceTimeScale(10);
        }

        private void UpdateSunset(float sunsetProgress, float sunsetDuration)
        {
            float timeFactor = Mathf.Lerp(0, 1, sunsetProgress / sunsetDuration);
            _material.SetFloat(TimeFactor, timeFactor);
            _material.SetFloat(TimeFactorNight, timeFactor);

            if (!_isForcingNight)
                ChangeSequenceTimeScale(0);
        }

        private void UpdateDay(float dayProgress, float dayInPercent)
        {
            float timeFactor = Mathf.Lerp(0, 1, dayProgress / dayInPercent);
            _material.SetFloat(TimeFactorDay, timeFactor);

            if (!_isForcingNight)
                ChangeSequenceTimeScale(1);
        }


        public void ForceNight(Action onCompleted = null)
        {
            if (_dayTween == null || !_dayTween.IsActive())
                return;

            float remainingDay = 0;

            if (_isForcingNight == false)
            {
                float elapsedDay = _dayTween.Elapsed();
                remainingDay = Mathf.Max(0f, _dayTime - elapsedDay);
            }

            _isForcingNight = true;

            if (remainingDay <= 0.01f)
                return;

            float speedMultiplier = remainingDay / 2f;

            _sequence.timeScale = speedMultiplier;
            _nightTween.OnStart(() =>
            {
                onCompleted?.Invoke();
                _sequence.timeScale = 1;
                _isForcingNight = false;
            });
        }


        public void ForceDay(Action onCompleted = null)
        {
            if (_nightTween == null || !_nightTween.IsActive())
                return;

            float remainingNight = 0;

            if (_isForcingDay == false)
            {
                float elapsedNight = _nightTween.Elapsed();
                remainingNight = Mathf.Max(0f, _nightTime - elapsedNight);
            }

            _isForcingDay = true;

            if (remainingNight <= 0.01f)
                return;

            float speedMultiplier = remainingNight / 2f;

            _sequence.timeScale = speedMultiplier;
            _dayTween.OnStart(() =>
            {
                onCompleted?.Invoke();

                _sequence.timeScale = 1;
                _isForcingDay = false;

                UpdateNightLights(_nightTime, _nightTime);
            });
        }

        public void UpdateLightIntensity(LightTypeId lightType, float value)
        {
            switch (lightType)
            {
                case LightTypeId.SunNormal:
                    _sunNormalLight.intensity = value;
                    break;
                case LightTypeId.SunBGNormal:
                    _sunNormalBGLight.intensity = value;
                    break;
                case LightTypeId.SunMGNormal:
                    _sunNormalMGLight.intensity = value;
                    break;

                case LightTypeId.SunMask:
                    _sunMaskLight.intensity = value;
                    break;
                case LightTypeId.SunBGMask:
                    _sunMaskBGLight.intensity = value;
                    break;
                case LightTypeId.SunMGMask:
                    _sunMaskMGLight.intensity = value;
                    break;

                case LightTypeId.MoonNormal:
                    _moonNormalLight.intensity = value;
                    break;
                case LightTypeId.MoonBGNormal:
                    _moonNormalBGLight.intensity = value;
                    break;
                case LightTypeId.MoonMGNormal:
                    _moonNormalMGLight.intensity = value;
                    break;

                case LightTypeId.MoonMask:
                    _moonMaskLight.intensity = value;
                    break;
                case LightTypeId.MoonBGMask:
                    _moonMaskBGLight.intensity = value;
                    break;
                case LightTypeId.MoonMGMask:
                    _moonMaskMGLight.intensity = value;
                    break;

                case LightTypeId.Cristal:
                    if (_cristalLight != null)
                        _cristalLight.intensity = value;
                    break;

                case LightTypeId.Global:
                    _globalLight.intensity = value;
                    break;
                case LightTypeId.GlobalBG:
                    _globalBGLight.intensity = value;
                    break;
                case LightTypeId.GlobalMG:
                    _globalMGLight.intensity = value;
                    break;
            }
        }

        public float GetLightIntenisty(LightTypeId lightType)
        {
            switch (lightType)
            {
                case LightTypeId.SunNormal:
                    return _sunNormalLight.intensity;
                case LightTypeId.SunBGNormal:
                    return _sunNormalBGLight.intensity;
                case LightTypeId.SunMGNormal:
                    return _sunNormalMGLight.intensity;

                case LightTypeId.SunMask:
                    return _sunMaskLight.intensity;
                case LightTypeId.SunBGMask:
                    return _sunMaskBGLight.intensity;
                case LightTypeId.SunMGMask:
                    return _sunMaskMGLight.intensity;

                case LightTypeId.MoonNormal:
                    return _moonNormalLight.intensity;
                case LightTypeId.MoonBGNormal:
                    return _moonNormalBGLight.intensity;
                case LightTypeId.MoonMGNormal:
                    return _moonNormalMGLight.intensity;

                case LightTypeId.MoonMask:
                    return _moonMaskLight.intensity;
                case LightTypeId.MoonBGMask:
                    return _moonMaskBGLight.intensity;
                case LightTypeId.MoonMGMask:
                    return _moonMaskMGLight.intensity;

                case LightTypeId.Cristal:
                    return _cristalLight != null ? _cristalLight.intensity : 0;

                case LightTypeId.Global:
                    return _globalLight.intensity;
                case LightTypeId.GlobalBG:
                    return _globalBGLight.intensity;
                case LightTypeId.GlobalMG:
                    return _globalMGLight.intensity;
            }

            return 0;
        }

        public void ToggleLight(LightTypeId lightType, bool value)
        {
            switch (lightType)
            {
                case LightTypeId.SunNormal:
                    _sunNormalLight.gameObject.SetActive(value);
                    break;
                case LightTypeId.SunBGNormal:
                    _sunNormalBGLight.gameObject.SetActive(value);
                    break;
                case LightTypeId.SunMGNormal:
                    _sunNormalMGLight.gameObject.SetActive(value);
                    break;

                case LightTypeId.SunMask:
                    _sunMaskLight.gameObject.SetActive(value);
                    break;
                case LightTypeId.SunBGMask:
                    _sunMaskBGLight.gameObject.SetActive(value);
                    break;
                case LightTypeId.SunMGMask:
                    _sunMaskMGLight.gameObject.SetActive(value);
                    break;

                case LightTypeId.MoonNormal:
                    _moonNormalLight.gameObject.SetActive(value);
                    break;
                case LightTypeId.MoonBGNormal:
                    _moonNormalBGLight.gameObject.SetActive(value);
                    break;
                case LightTypeId.MoonMGNormal:
                    _moonNormalMGLight.gameObject.SetActive(value);
                    break;

                case LightTypeId.MoonMask:
                    _moonMaskLight.gameObject.SetActive(value);
                    break;
                case LightTypeId.MoonBGMask:
                    _moonMaskBGLight.gameObject.SetActive(value);
                    break;
                case LightTypeId.MoonMGMask:
                    _moonMaskMGLight.gameObject.SetActive(value);
                    break;

                case LightTypeId.Global:
                    _globalLight.gameObject.SetActive(value);
                    break;
                case LightTypeId.GlobalBG:
                    _globalBGLight.gameObject.SetActive(value);
                    break;
                case LightTypeId.GlobalMG:
                    _globalMGLight.gameObject.SetActive(value);
                    break;
            }
        }

        public void FreezTime()
        {
            _sequence.Pause();
            _dayTween.Pause();
            _nightTween.Pause();
        }


        public void UnFreezTime()
        {
            _sequence.Play();
            _dayTween.Play();
            _nightTween.Play();
        }


        private void ChangeSequenceTimeScale(float value)
        {
            if (Mathf.Approximately(value, _sequence.timeScale))
                return;

            _sequence.timeScale = value;
        }

#if UNITY_EDITOR


        public void UpdateDayLightsEditor(float currentDayTime, float dayTime) =>
            UpdateDayLights(currentDayTime, dayTime);

        public void UpdateGlobalLightsEditor(float currentTime, float dayCycleTime) =>
            UpdateGlobalLights(currentTime, dayCycleTime);

        public void UpdateNightLightsEditor(float currentNightTime, float nightTime) =>
            UpdateNightLights(currentNightTime, nightTime);

        public void UpdateSunriseEditor(float sunriseProgress, float halfDayTime) =>
            UpdateSunrise(sunriseProgress, halfDayTime);

        public void UpdateSunsetEditor(float sunsetProgress, float halfDayTime) =>
            UpdateSunset(sunsetProgress, halfDayTime);

        public void UpdateDayEditor(float dayProgress, float dayInPercent) =>
            UpdateDay(dayProgress, dayInPercent);

        public void DayCycleRotationEditor(float value)
        {
            float angle = Mathf.Lerp(90f, -270f, value);
            _dayCycleTransform.rotation = Quaternion.Euler(0, 0, angle);
        }

        public void ActiveStreetLightEditor(bool value) =>
            ActiveStreetLight(value);
#endif
    }
}