using DG.Tweening;
using Infastructure.Services.Tutorial;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Windows
{
    public class FinishTutorialWindow : WindowBase
    {
        [SerializeField] private Transform _container;
        [SerializeField] private Image _image;

        private ITutorialCheckerService _tutorialCheckerService;

        [Inject]
        public void Construct(ITutorialCheckerService tutorialCheckerService) =>
            _tutorialCheckerService = tutorialCheckerService;

        protected override void Initialize()
        {
            base.Initialize();

            _container.localScale = Vector3.zero;

            _container.DOScale(Vector3.one, 1);

            _image.DOFade(0, 2)
                .SetDelay(3)
                .OnComplete(() =>
                {
                    _tutorialCheckerService.TutorialStarted = false;
                    Destroy(gameObject);
                });
            ;
        }
    }
}