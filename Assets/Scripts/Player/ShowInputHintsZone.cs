using UI.GameplayUI;
using UI.GameplayUI.TowerSelectionUI;
using UnityEngine;

namespace Player
{
    public class ShowInputHintsZone : MonoBehaviour
    {
        [SerializeField] private PlayerMove _playerMove;
        [SerializeField] private ObserverTrigger _observerTrigger;

        private void Start()
        {
            _observerTrigger.OnTriggerEnter += Enter;
            _observerTrigger.OnTriggerExit += Exit;

            _playerMove.AccelerationButtonUpHappened += Enter;
        }

        private void OnDestroy()
        {
            _observerTrigger.OnTriggerEnter -= Enter;
            _observerTrigger.OnTriggerExit -= Exit;

            _playerMove.AccelerationButtonUpHappened -= Enter;
        }

        private void Enter()
        {
            if (_playerMove.AccelerationPressedWithMove || _observerTrigger.CurrentCollider == null)
                return;

            if (_observerTrigger.CurrentCollider.TryGetComponent(out HintsDisplayBase hintsDisplayBase))
            {
                if (hintsDisplayBase is not TowerHintsDisplay)
                    hintsDisplayBase.ShowHints();
            }
        }

        private void Exit()
        {
            if (_observerTrigger.CurrentCollider.TryGetComponent(out HintsDisplayBase HintsDisplayBase))
                HintsDisplayBase.HideHints();
        }
    }
}