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

        [Inject]
        public void Construct(IStateMachine stateMachine) =>
            _stateMachine = stateMachine;

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
            StartGame();
        }

        private void TutorialOpen()
        {
            StartGame();
        }

        private void StartGame() =>
            _stateMachine.Enter<LoadLevelState>();
    }
}