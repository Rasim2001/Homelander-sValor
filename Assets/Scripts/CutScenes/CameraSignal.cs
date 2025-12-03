using Cinemachine;
using DG.Tweening;
using UnityEngine;

namespace CutScenes
{
    public class CameraSignal
    {
        private CinemachineVirtualCamera _cutSceneNearCamera;
        private readonly CinemachineVirtualCamera _timelineCamera;
        private readonly Transform _player;

        private readonly float _duration = 0.1f;
        private readonly float _randomness = 90;
        private readonly int _vibrato = 15;

        private float _strength = 0.05f;

        private Tween _shakeTween;
        private GameObject _emptyObject;

        public CameraSignal(Transform player, CinemachineVirtualCamera cutSceneNearCamera,
            CinemachineVirtualCamera timelineCamera)
        {
            _player = player;
            _cutSceneNearCamera = cutSceneNearCamera;
            _timelineCamera = timelineCamera;
        }

        public void StopCamera() =>
            _cutSceneNearCamera.gameObject.SetActive(true);

        public void ActivateTimelineCamera()
        {
            _timelineCamera.gameObject.SetActive(true);

            _shakeTween = _timelineCamera.transform
                .DOShakePosition(_duration, _strength, _vibrato, _randomness)
                .SetEase(Ease.InOutBounce)
                .SetDelay(0.25f)
                .OnStart(() => _timelineCamera.Follow = null)
                .SetLoops(-1, LoopType.Yoyo)
                .OnStepComplete(() => _strength += 0.05f);
        }

        public void Shake()
        {
            _emptyObject = new GameObject();
            _emptyObject.transform.position = _player.position;
            _cutSceneNearCamera.Follow = _emptyObject.transform;

            /*_shakeTween = _cutSceneNearCamera.transform
                .DOShakePosition(_duration, _strength, _vibrato, _randomness)
                .SetEase(Ease.InOutBounce)
                .SetDelay(0.25f)
                .OnStart(() => _cutSceneNearCamera.Follow = null)
                .SetLoops(-1, LoopType.Yoyo)
                .OnStepComplete(() => _strength += 0.05f);*/
        }

        public void CancelShake()
        {
            Tween _orthoTween = DOTween.To(
                () => _cutSceneNearCamera.m_Lens.OrthographicSize,
                x => _cutSceneNearCamera.m_Lens.OrthographicSize = x,
                5.25f,
                0.2f
            ).SetEase(Ease.InOutBounce);

            Tween _orthoTween2 = DOTween.To(
                () => _cutSceneNearCamera.m_Lens.OrthographicSize,
                x => _cutSceneNearCamera.m_Lens.OrthographicSize = x,
                5.5f,
                0.05f
            ).SetEase(Ease.InOutBounce);


            DOTween.Sequence()
                .Append(_orthoTween)
                .Append(_orthoTween2);

            _shakeTween.Kill();
        }

        public void MoveToPlayer()
        {
            _cutSceneNearCamera.gameObject.SetActive(false);
            _cutSceneNearCamera.Follow = null;

            _timelineCamera.gameObject.SetActive(false);
            _timelineCamera.Follow = null;

            Object.Destroy(_emptyObject);
        }
    }
}