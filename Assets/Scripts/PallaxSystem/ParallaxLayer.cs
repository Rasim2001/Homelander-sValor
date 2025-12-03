using Cinemachine;
using UnityEngine;

namespace PallaxSystem
{
    public class ParallaxLayer : MonoBehaviour
    {
        [SerializeField] float _multiplierX = 0.1f;
        [SerializeField] float _multiplierY = 0.1f;

        private Vector3 _lastCameraPosition;
        private Transform _cameraTransform;

        private void Awake() =>
            _cameraTransform = Camera.main.transform;

        private void Start() =>
            CinemachineCore.CameraUpdatedEvent.AddListener(UpdateParallax);

        private void OnDestroy() =>
            CinemachineCore.CameraUpdatedEvent.RemoveListener(UpdateParallax);

        private void UpdateParallax(CinemachineBrain arg0)
        {
            Vector3 deltaMovement = _cameraTransform.position - _lastCameraPosition;
            Vector3 targetPosition = transform.position +
                                     new Vector3(deltaMovement.x * _multiplierX, deltaMovement.y * _multiplierY);
            transform.position = targetPosition;

            _lastCameraPosition = _cameraTransform.position;
        }
    }
}