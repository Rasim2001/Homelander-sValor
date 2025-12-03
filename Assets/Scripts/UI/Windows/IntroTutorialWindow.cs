using Infastructure.Services.Tutorial;
using Infastructure.States;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Windows
{
    public class IntroTutorialWindow : WindowBase
    {
        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _skipButton;

        private IStateMachine _stateMachine;
        private ITutorialCheckerService _tutorialCheckerService;

        [Inject]
        public void Constuct(IStateMachine stateMachine, ITutorialCheckerService tutorialCheckerService)
        {
            _stateMachine = stateMachine;
            _tutorialCheckerService = tutorialCheckerService;
        }

        protected override void SubscribeUpdates()
        {
            base.SubscribeUpdates();

            _continueButton.onClick.AddListener(TutorialOpen);
            _skipButton.onClick.AddListener(Skip);
        }

        protected override void Cleanup()
        {
            base.Cleanup();

            _continueButton.onClick.RemoveListener(TutorialOpen);
            _skipButton.onClick.RemoveListener(Skip);
        }

        private void Skip()
        {
            _tutorialCheckerService.TutorialStarted = false;

            StartGame();
        }

        private void TutorialOpen()
        {
            _tutorialCheckerService.TutorialStarted = true;

            StartGame();
        }

        private void StartGame() =>
            _stateMachine.Enter<LoadLevelState>();
    }
}