using Infastructure.Services.Window;
using Infastructure.StaticData.Windows;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI
{
    public class OpenWindowButton : MonoBehaviour
    {
        public Button Button;
        public WindowId WindowId;

        private IWindowService _windowService;

        [Inject]
        public void Construct(IWindowService windowService) =>
            _windowService = windowService;

        private void Awake() =>
            Button.onClick.AddListener(Open);

        private void OnDestroy() =>
            Button.onClick.RemoveListener(Open);

        private void Open()
        {
            if (WindowId != WindowId.Unknow)
                _windowService.Open(WindowId);
        }
    }
}