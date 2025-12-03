using Player;
using UnityEngine;

namespace UI.GameplayUI.CristalUI
{
    public class CristalMove : ICristalMove
    {
        private readonly PlayerMove _playerMove;
        private readonly Transform _playerTransform;
        private readonly Transform _cristalTransform;

        private readonly float _followSpeed = 4f;
        private readonly float _waveAmplitude = 0.1f;
        private readonly float _waveFrequency = 4f;

        private float _waveTimer = 0f;

        public CristalMove(PlayerMove playerMove, Transform cristalTransform)
        {
            _playerMove = playerMove;

            _playerTransform = playerMove.transform;
            _cristalTransform = cristalTransform;
        }

        public void Update()
        {
            _waveTimer += Time.deltaTime * _waveFrequency;

            Vector2 targetPosition =
                new Vector2(_playerTransform.position.x, _playerTransform.position.y + 2);

            float waveOffsetY = Mathf.Sin(_waveTimer) * _waveAmplitude;
            targetPosition.y += waveOffsetY;

            _cristalTransform.position =
                Vector2.Lerp(_cristalTransform.position, targetPosition,
                    (_followSpeed + _playerMove.Speed) * Time.deltaTime);
        }

        public void Update(Vector2 targetPosition)
        {
            _waveTimer += Time.deltaTime * _waveFrequency;

            float waveOffsetY = Mathf.Sin(_waveTimer) * _waveAmplitude;
            targetPosition.y += waveOffsetY;

            _cristalTransform.position =
                Vector2.Lerp(_cristalTransform.position, targetPosition,
                    (_followSpeed + _playerMove.Speed) * Time.deltaTime);
        }
    }
}