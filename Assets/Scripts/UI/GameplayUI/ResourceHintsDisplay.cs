using Infastructure.Services.ResourceLimiter;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Zenject;

namespace UI.GameplayUI
{
    public class ResourceHintsDisplay : HintsDisplayBase
    {
        [SerializeField] private GameObject _keyboard;
        [SerializeField] private Light2D _light2D;

        private IResourceLimiterService _resourceLimiterService;

        [Inject]
        public void Construct(IResourceLimiterService resourceLimiterService) =>
            _resourceLimiterService = resourceLimiterService;

        protected override void Show(bool value)
        {
            if (!_resourceLimiterService.IsActive(_orderMarker))
                return;

            _keyboard.SetActive(value);
            _light2D.enabled = value;
        }
    }
}