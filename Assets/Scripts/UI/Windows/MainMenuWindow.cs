using DG.Tweening;
using Infastructure.Services.QuitGameApplicationService;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Windows
{
    public class MainMenuWindow : WindowBase
    {
        [SerializeField] private Button _clearProgressButton;
        [SerializeField] private Button _exitButton;
        [SerializeField] private Transform _container;

        private IQuitGameService _quitGameService;

        [Inject]
        public void Construct(IQuitGameService quitGameService) =>
            _quitGameService = quitGameService;

        protected override void Initialize()
        {
            if (_container != null)
            {
                _container.localScale = Vector3.zero;
                _container.DOScale(Vector3.one, 1);
            }
        }

        protected override void SubscribeUpdates()
        {
            base.SubscribeUpdates();

            _exitButton.onClick.AddListener(Exit);
        }

        protected override void Cleanup()
        {
            base.Cleanup();

            _exitButton.onClick.RemoveListener(Exit);

            _container.DOKill();
        }


        private void Exit() =>
            _quitGameService.QuitGame();
    }
}