using Cinemachine;
using UnityEngine;

namespace CameraManagement
{
    public class UICamera : MonoBehaviour
    {
        [SerializeField] private Camera _uiCamera;
        [SerializeField] private Camera _mainCamera;

        private void Start() =>
            CinemachineCore.CameraUpdatedEvent.AddListener(UpdateUICamera);

        private void OnDestroy() =>
            CinemachineCore.CameraUpdatedEvent.RemoveListener(UpdateUICamera);

        private void UpdateUICamera(CinemachineBrain arg0) =>
            _uiCamera.orthographicSize = _mainCamera.orthographicSize;
    }
}