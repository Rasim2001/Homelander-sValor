using DG.Tweening;
using Infastructure.Services.Tutorial.TutorialProgress;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Windows
{
    public class FinishTutorialWindow : WindowBase
    {
        [SerializeField] private Transform _container;
        [SerializeField] private Image _image;

        private ITutorialProgressService _tutorialProgressService;

        [Inject]
        public void Construct(ITutorialProgressService tutorialProgressService) =>
            _tutorialProgressService = tutorialProgressService;

        protected override void Initialize()
        {
            base.Initialize();

            _container.localScale = Vector3.zero;

            _container.DOScale(Vector3.one, 1);

            _image.DOFade(0, 2)
                .SetDelay(3)
                .OnComplete(() =>
                {
                    _tutorialProgressService.TutorialStarted = false;
                    Destroy(gameObject);
                });
            ;
        }
    }
}