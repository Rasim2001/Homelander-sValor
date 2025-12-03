using Cinemachine;
using Player;
using UnityEngine;

namespace CameraManagement
{
    public class CinemachineFollow : MonoBehaviour
    {
        public CinemachineVirtualCamera FarCamera;
        public CinemachineVirtualCamera NearCamera;
        public CinemachineVirtualCamera BonfireCamera;
        public CinemachineVirtualCamera CutSceneCamera;

        public void Initialize(GameObject playerObject)
        {
            CameraFollow cameraFollow = playerObject.GetComponentInChildren<CameraFollow>();
            PlayerFlip playerFlip = playerObject.GetComponent<PlayerFlip>();
            PlayerMove playerMove = playerObject.GetComponent<PlayerMove>();

            FarCamera.Follow = cameraFollow.transform;
            NearCamera.Follow = cameraFollow.transform;
            CutSceneCamera.Follow = cameraFollow.transform;

            cameraFollow.Initialize(playerFlip, playerMove, NearCamera);

            BonfireCamera.gameObject.SetActive(false);
            CutSceneCamera.gameObject.SetActive(false);
        }

        public void FocusOnMainFlag() =>
            BonfireCamera.gameObject.SetActive(true);

        public void SetNearCamera() =>
            SetDefaultCamera(true);

        public void SetFarCamera() =>
            SetDefaultCamera(false);

        public CinemachineVirtualCamera GetCutSceneNearCamera() =>
            CutSceneCamera;

        public CinemachineVirtualCamera GetNearCamera() =>
            NearCamera;

        public CinemachineVirtualCamera GetFarCamera() =>
            FarCamera;


        private void SetDefaultCamera(bool isNearCameraActive)
        {
            if (NearCamera == null || FarCamera == null)
                return;

            NearCamera.gameObject.SetActive(isNearCameraActive);
            FarCamera.gameObject.SetActive(!isNearCameraActive);
        }
    }
}