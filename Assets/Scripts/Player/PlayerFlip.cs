using Infastructure.Services.CameraFocus;
using Infastructure.Services.InputPlayerService;
using UnityEngine;
using Zenject;

namespace Player
{
    public class PlayerFlip : MonoBehaviour
    {
        [SerializeField] private PlayerMove _playerMove;
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private IInputService _inputService;
        private ICameraFocusService _cameraFocusService;

        [Inject]
        public void Construct(IInputService inputService, ICameraFocusService cameraFocusService)
        {
            _cameraFocusService = cameraFocusService;
            _inputService = inputService;
        }

        public int FlipValue() =>
            _spriteRenderer.flipX ? 1 : -1;

        public bool FlipBoolValue() =>
            _spriteRenderer.flipX;

        private void Update()
        {
            if (!_playerMove.IsMoving() || !_playerMove.enabled || _cameraFocusService.PlayerDefeated)
                return;

            if (_inputService.AxisX > 0)
                _spriteRenderer.flipX = false;
            else if (_inputService.AxisX < 0)
                _spriteRenderer.flipX = true;
        }
    }
}