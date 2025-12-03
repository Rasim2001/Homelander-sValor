using Cinemachine;
using Player;
using UnityEngine;

namespace CameraManagement
{
    public class CameraFollow : MonoBehaviour
    {
        private PlayerFlip _flip;
        private PlayerMove _move;

        private int lastFlipValue;
        private float _changestTime;

        private bool _isLerped;
        private bool _isLerped2;
        private Vector3 _targetPosition;

        private float _offset;
        private CinemachineVirtualCamera _nearCamera;

        public void Initialize(PlayerFlip flip, PlayerMove move, CinemachineVirtualCamera nearCamera)
        {
            _flip = flip;
            _move = move;
            _nearCamera = nearCamera;
        }

        private void Start() =>
            _changestTime = 5;


        private void Update()
        {
            if (_flip == null)
                return;

            if (!_nearCamera.isActiveAndEnabled)
                KeepCameraOnPlayer();
            else
                UpdateFollow();
        }

        private void KeepCameraOnPlayer()
        {
            float targetPosition = _flip.transform.position.x;

            transform.position = Vector3.MoveTowards(transform.position,
                new Vector3(targetPosition, transform.position.y, -10),
                Time.deltaTime * _move.Speed);
        }

        private void UpdateFollow()
        {
            if (lastFlipValue == _flip.FlipValue())
            {
                if (_changestTime > 0)
                {
                    _changestTime -= Time.deltaTime;

                    if (_isLerped2)
                        transform.position = new Vector3(_flip.transform.position.x, transform.position.y);


                    if (!_isLerped2)
                    {
                        float targetPosition = _flip.transform.position.x;

                        transform.position = Vector3.MoveTowards(transform.position,
                            new Vector3(targetPosition, transform.position.y, -10),
                            Time.deltaTime * _move.Speed);

                        if (Mathf.Abs(transform.position.x - targetPosition) < 0.01f)
                            _isLerped2 = true;
                    }
                }
                else
                {
                    _offset = 3 * -_flip.FlipValue();

                    if (_isLerped)
                        transform.position =
                            new Vector3(_flip.transform.position.x + _offset, transform.position.y);


                    if (!_isLerped)
                    {
                        float targetPosition = _flip.transform.position.x + _offset;
                        float localSpeed = _move.Speed;

                        if (!_move.IsMoving())
                            localSpeed = 1;


                        transform.position = Vector3.MoveTowards(transform.position,
                            new Vector3(targetPosition, transform.position.y),
                            Time.deltaTime /* * localSpeed*/);

                        if (Mathf.Abs(transform.position.x - targetPosition) < 0.01f)
                            _isLerped = true;
                    }
                }
            }
            else
            {
                _changestTime = 5;
                _offset = 0;
                _isLerped = false;
                _isLerped2 = false;
            }


            lastFlipValue = _flip.FlipValue();
        }
    }
}