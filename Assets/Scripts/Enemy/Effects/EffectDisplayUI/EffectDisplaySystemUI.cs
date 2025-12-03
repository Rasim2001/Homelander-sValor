using System;
using System.Collections.Generic;
using Infastructure;
using Infastructure.Services.AssetProvider;
using Infastructure.StaticData.EffectsUI;
using Infastructure.StaticData.StaticDataService;
using UnityEngine;

namespace Enemy.Effects.EffectDisplayUI
{
    public class EffectDisplaySystemUI
    {
        private readonly Dictionary<Type, EffectBarUI> _dictionary = new Dictionary<Type, EffectBarUI>();

        private readonly IStaticDataService _staticDataService;
        private readonly IAssetProviderService _assetProviderService;


        public EffectDisplaySystemUI(IStaticDataService staticDataService, IAssetProviderService assetProviderService)
        {
            _staticDataService = staticDataService;
            _assetProviderService = assetProviderService;
        }

        public void Initialize(Transform parentEffects)
        {
            EffectsUIStaticData enemyEffectStaticData = _staticDataService.EnemyEffectStaticData;

            foreach (EffectEntry effectEntry in enemyEffectStaticData.Effects)
            {
                GameObject effectItemUI =
                    _assetProviderService.Instantiate(AssetsPath.EnemyEffectUIItem, parentEffects);

                EffectBarUI effectBarUI = effectItemUI.GetComponent<EffectBarUI>();
                effectBarUI.SetEffectSprite(effectEntry.EffectSprite);

                _dictionary.Add(effectEntry.EffectType, effectBarUI);
            }
        }

        public void UpdateProgressBarUI()
        {
            foreach (EffectBarUI effectBarUI in _dictionary.Values)
                effectBarUI.UpdateEffectBarUI();
        }


        public void SetDuration(Type type, float duration)
        {
            if (_dictionary.TryGetValue(type, out EffectBarUI effectObject))
                effectObject.SetEffectBarTime(duration);
        }

        public void Show(Type type)
        {
            if (_dictionary.TryGetValue(type, out EffectBarUI effectObject))
                SetActive(effectObject.gameObject, true);
        }


        public void Hide(Type type)
        {
            if (_dictionary.TryGetValue(type, out EffectBarUI effectObject))
                SetActive(effectObject.gameObject, false);
        }

        public void HideAll()
        {
            foreach (EffectBarUI effectUI in _dictionary.Values)
                SetActive(effectUI.gameObject, false);
        }


        private void SetActive(GameObject icon, bool value) =>
            icon.SetActive(value);
    }
}