using CutScenes;
using DayCycle;
using Fog;
using Infastructure.Services.ResourceLimiter;
using Infastructure.Services.SafeBuildZoneTracker;
using Infastructure.StaticData.Forest;
using Infastructure.StaticData.Lights;
using Infastructure.StaticData.StaticDataService;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Zenject;

namespace Infastructure.Services.Forest
{
    public class ForestTransitionService : IForestTransitionService, ITickable
    {
        private readonly IResourceLimiterService _resourceLimiter;
        private readonly IStaticDataService _staticDataService;
        private readonly ISafeBuildZone _safeBuildZone;
        private readonly ICristalTimeline _cristalTimeline;
        private readonly Volume _globalVolume;

        private Transform _playerTransform;
        private Transform _leftResource;

        private Transform _rightResource;
        private Vignette _vignette;
        private DayCycleUpdater _dayCycleUpdater;
        private LightStaticData _lightStaticData;
        private ForestTransitionStaticData _forestTransitionData;
        private FogUI _fogUI;

        private float _resultTransitionLerp;

        public ForestTransitionService(
            IResourceLimiterService resourceLimiter,
            IStaticDataService staticDataService,
            ISafeBuildZone safeBuildZone,
            Volume globalVolume,
            ICristalTimeline cristalTimeline)
        {
            _resourceLimiter = resourceLimiter;
            _globalVolume = globalVolume;
            _cristalTimeline = cristalTimeline;
            _staticDataService = staticDataService;
            _safeBuildZone = safeBuildZone;
        }

        public void Initialize(Transform playerTransform)
        {
            _playerTransform = playerTransform;

            if (_globalVolume.profile.TryGet(out Vignette vignette))
                _vignette = vignette;

            Camera camera = Camera.main;

            _dayCycleUpdater = camera.GetComponentInChildren<DayCycleUpdater>();
            _fogUI = camera.GetComponentInChildren<FogUI>();

            _lightStaticData = _staticDataService.LightStaticData;
            _forestTransitionData = _staticDataService.ForestTransitionStaticData;
        }

        public void SubscribeUpdates() =>
            _resourceLimiter.OnResourceChanged += ChangeEnteredResource;

        public void Cleanup() =>
            _resourceLimiter.OnResourceChanged -= ChangeEnteredResource;


        public void Tick()
        {
            if (!IsPlayerValid())
                return;

            float progress = CalculateTransitionProgress();

            if (_forestTransitionData.UseVignetteCurve)
            {
                if (!_safeBuildZone.IsNight)
                    UpdateCristalIntensityDay(progress);
                else
                    UpdateCristalIntensityNight();

                UpdateVignetteIntensity(progress);
            }

            if (_forestTransitionData.UseLightCurve)
                UpdateDayCycleIntensity(progress);

            if (_forestTransitionData.UseFogCurve)
                UpdateFogUIIntensity(progress);
        }

        private void UpdateFogUIIntensity(float progress)
        {
            float currentIntensity = _fogUI.GetAlpha();
            float transitionSpeed = Time.deltaTime * _forestTransitionData.TransitionSpeed;
            float curveProgress = _forestTransitionData.FogCurveUI.Evaluate(progress);

            float alpha = Mathf.Lerp(currentIntensity, curveProgress, transitionSpeed);

            _fogUI.ChangeIntensity(alpha);
        }

        private void UpdateDayCycleIntensity(float progress)
        {
            float invertedProgress = 1f - progress;
            float transitionSpeed = Time.deltaTime * _forestTransitionData.TransitionSpeed;

            float currentSunNormal = _dayCycleUpdater.GetLightIntenisty(LightTypeId.SunNormal);
            float currentSunBGNormal = _dayCycleUpdater.GetLightIntenisty(LightTypeId.SunBGNormal);
            float currentSunMGNormal = _dayCycleUpdater.GetLightIntenisty(LightTypeId.SunMGNormal);

            float currentSunMask = _dayCycleUpdater.GetLightIntenisty(LightTypeId.SunMask);
            float currentSunBGMask = _dayCycleUpdater.GetLightIntenisty(LightTypeId.SunBGMask);
            float currentSunMGMask = _dayCycleUpdater.GetLightIntenisty(LightTypeId.SunMGMask);

            float currentMoonNormal = _dayCycleUpdater.GetLightIntenisty(LightTypeId.MoonNormal);
            float currentMoonBGNormal = _dayCycleUpdater.GetLightIntenisty(LightTypeId.MoonBGNormal);
            float currentMoonMGNormal = _dayCycleUpdater.GetLightIntenisty(LightTypeId.MoonMGNormal);
            
            float currentMoonMask = _dayCycleUpdater.GetLightIntenisty(LightTypeId.MoonMask);
            float currentMoonBGMask = _dayCycleUpdater.GetLightIntenisty(LightTypeId.MoonBGMask);
            float currentMoonMGMask = _dayCycleUpdater.GetLightIntenisty(LightTypeId.MoonMGMask);

            float targetSunNormal = _lightStaticData.SunNormalLightIntensity * _forestTransitionData.LightCurve.Evaluate(invertedProgress);
            float targetSunBGNormal = _lightStaticData.SunNormalBGLightIntensity * _forestTransitionData.LightCurve.Evaluate(invertedProgress);
            float targetSunMGNormal = _lightStaticData.SunNormalMGLightIntensity * _forestTransitionData.LightCurve.Evaluate(invertedProgress);

            float targetSunMask = _lightStaticData.SunMaskLightIntensity * _forestTransitionData.LightCurve.Evaluate(invertedProgress);
            float targetBGSunMask = _lightStaticData.SunMaskBGLightIntensity * _forestTransitionData.LightCurve.Evaluate(invertedProgress);
            float targetMGSunMask = _lightStaticData.SunMaskMGLightIntensity * _forestTransitionData.LightCurve.Evaluate(invertedProgress);

            float targetMoonNormal = _lightStaticData.MoonNormalLightIntensity * _forestTransitionData.LightCurve.Evaluate(invertedProgress);
            float targetMoonBGNormal = _lightStaticData.MoonNormalBGLightIntensity * _forestTransitionData.LightCurve.Evaluate(invertedProgress);
            float targetMoonMGNormal = _lightStaticData.MoonNormalMGLightIntensity * _forestTransitionData.LightCurve.Evaluate(invertedProgress);
            
            float targetMoonMask = _lightStaticData.MoonMaskLightIntensity * _forestTransitionData.LightCurve.Evaluate(invertedProgress);
            float targetMoonBGMask = _lightStaticData.MoonMaskBGLightIntensity * _forestTransitionData.LightCurve.Evaluate(invertedProgress);
            float targetMoonMGMask = _lightStaticData.MoonMaskMGLightIntensity * _forestTransitionData.LightCurve.Evaluate(invertedProgress);

            float sunNormal = Mathf.Lerp(currentSunNormal, targetSunNormal, transitionSpeed);
            float sunBGNormal = Mathf.Lerp(currentSunBGNormal, targetSunBGNormal, transitionSpeed);
            float sunMGNormal = Mathf.Lerp(currentSunMGNormal, targetSunMGNormal, transitionSpeed);

            float sunMask = Mathf.Lerp(currentSunMask, targetSunMask, transitionSpeed);
            float sunBGMask = Mathf.Lerp(currentSunBGMask, targetBGSunMask, transitionSpeed);
            float sunMGMask = Mathf.Lerp(currentSunMGMask, targetMGSunMask, transitionSpeed);

            float moonNormal = Mathf.Lerp(currentMoonNormal, targetMoonNormal, transitionSpeed);
            float moonBGNormal = Mathf.Lerp(currentMoonBGNormal, targetMoonBGNormal, transitionSpeed);
            float moonMGNormal = Mathf.Lerp(currentMoonMGNormal, targetMoonMGNormal, transitionSpeed);
            
            float moonMask = Mathf.Lerp(currentMoonMask, targetMoonMask, transitionSpeed);
            float moonBGMask = Mathf.Lerp(currentMoonBGMask, targetMoonBGMask, transitionSpeed);
            float moonMGMask = Mathf.Lerp(currentMoonMGMask, targetMoonMGMask, transitionSpeed);

            _dayCycleUpdater.UpdateLightIntensity(LightTypeId.SunNormal, sunNormal);
            _dayCycleUpdater.UpdateLightIntensity(LightTypeId.SunBGNormal, sunBGNormal);
            _dayCycleUpdater.UpdateLightIntensity(LightTypeId.SunMGNormal, sunMGNormal);

            _dayCycleUpdater.UpdateLightIntensity(LightTypeId.SunMask, Mathf.Clamp(sunMask, 0.01f, targetSunMask));
            _dayCycleUpdater.UpdateLightIntensity(LightTypeId.SunBGMask, Mathf.Clamp(sunBGMask, 0.01f, targetBGSunMask));
            _dayCycleUpdater.UpdateLightIntensity(LightTypeId.SunMGMask, Mathf.Clamp(sunMGMask, 0.01f, targetMGSunMask));

            _dayCycleUpdater.UpdateLightIntensity(LightTypeId.MoonNormal, moonNormal);
            _dayCycleUpdater.UpdateLightIntensity(LightTypeId.MoonBGNormal, moonBGNormal);
            _dayCycleUpdater.UpdateLightIntensity(LightTypeId.MoonMGNormal, moonMGNormal);

            _dayCycleUpdater.UpdateLightIntensity(LightTypeId.MoonMask, Mathf.Clamp(moonMask, 0.01f, targetMoonMask));
            _dayCycleUpdater.UpdateLightIntensity(LightTypeId.MoonBGMask, Mathf.Clamp(moonBGMask, 0.01f, targetMoonBGMask));
            _dayCycleUpdater.UpdateLightIntensity(LightTypeId.MoonMGMask, Mathf.Clamp(moonMGMask, 0.01f, targetMoonMGMask));
        }

        private void UpdateCristalIntensityDay(float progress)
        {
            float transitionSpeed = Time.deltaTime * _forestTransitionData.TransitionSpeed;

            float currentIntensity = _dayCycleUpdater.GetLightIntenisty(LightTypeId.Cristal);
            float curveProgress = _forestTransitionData.LightCurve.Evaluate(progress);
            float result = Mathf.Lerp(currentIntensity, curveProgress, transitionSpeed);

            _dayCycleUpdater.UpdateLightIntensity(LightTypeId.Cristal, result);
        }

        private void UpdateCristalIntensityNight()
        {
            float transitionSpeed = Time.deltaTime * _forestTransitionData.TransitionSpeed;

            float currentIntensity = _dayCycleUpdater.GetLightIntenisty(LightTypeId.Cristal);
            float result = Mathf.Lerp(currentIntensity, 1, transitionSpeed);

            _dayCycleUpdater.UpdateLightIntensity(LightTypeId.Cristal, result);
        }

        private bool IsPlayerValid() =>
            _playerTransform != null;

        private float CalculateTransitionProgress()
        {
            if (_playerTransform == null)
                return 0f;

            if (IsRightZone())
                return CalculateProgressForResource(_rightResource);
            if (IsLeftZone())
                return CalculateProgressForResource(_leftResource);

            return 0f;
        }

        private bool IsRightZone()
        {
            return _rightResource != null &&
                   _playerTransform.position.x > 0 &&
                   _playerTransform.position.x > _rightResource.position.x;
        }

        private bool IsLeftZone()
        {
            return _leftResource != null &&
                   _playerTransform.position.x < 0 &&
                   _playerTransform.position.x < _leftResource.position.x;
        }

        private void UpdateVignetteIntensity(float targetIntensity)
        {
            float curveProgress;

            if (_cristalTimeline.IsPlaying)
                curveProgress = 0;
            else
                curveProgress = _forestTransitionData.VignetteCurve.Evaluate(targetIntensity);

            float currentIntensity = _vignette.intensity.value;
            float transitionSpeed = Time.deltaTime * _forestTransitionData.TransitionSpeed;

            _vignette.intensity.value =
                Mathf.Lerp(currentIntensity, curveProgress * _forestTransitionData.VignetteValue, transitionSpeed);
        }

        private float CalculateProgressForResource(Transform resource)
        {
            float distance = Mathf.Abs(_playerTransform.position.x - resource.position.x);
            return Mathf.Clamp01(distance / _forestTransitionData.TransitionDistance);
        }


        private void ChangeEnteredResource(bool isRight, Transform resource)
        {
            if (isRight)
                _rightResource = resource;
            else
                _leftResource = resource;
        }
    }
}