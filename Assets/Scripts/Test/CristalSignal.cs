using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Test
{
    public class CristalSignal : MonoBehaviour
    {
        [SerializeField] private AnimationCurve _curve;

        [SerializeField] private Transform player;
        [SerializeField] private float minRadius = 3f;
        [SerializeField] private float maxRadius = 5f;
        [SerializeField] private float duration = 4f;
        [SerializeField] private int waypointCount = 10;

        [SerializeField] private float _heightStep = 0.3f;
        [SerializeField] private int _loops = 2;
        [SerializeField] private Ease _ease = Ease.Linear;

        [SerializeField] private Vector3 _playerOffset;

        private void Start() =>
            transform.localScale = Vector3.zero;


        public void StartOrbit() =>
            StartCoroutine(StartAnimationCoroutine());

        private IEnumerator StartAnimationCoroutine()
        {
            DoScaleAnimation();
            Tween doMoveTopAnimation = DoMoveTopAnimation();

            yield return new DOTweenCYInstruction.WaitForCompletion(doMoveTopAnimation);

            DoSpiralAnimation();
        }

        private void DoScaleAnimation() =>
            transform.DOScale(0.5f, 0.25f);

        private Tween DoMoveTopAnimation() =>
            transform.DOLocalMoveY(-0.78f, 2f);

        private void DoSpiralAnimation()
        {
            Vector3[] waypoints = GenerateWaypoints();


            transform.DOPath(waypoints, duration, PathType.CatmullRom, PathMode.Ignore, 10, Color.green)
                .SetEase(_ease)
                .SetRelative(false);
        }

        private Vector3[] GenerateWaypoints()
        {
            Vector3[] waypoints = new Vector3[waypointCount];
            waypoints[0] = transform.position;

            for (int i = 1; i < waypointCount; i++)
            {
                float angle = (i * (360f * _loops) / waypointCount) * Mathf.Deg2Rad;
                float radius = minRadius + (i * (maxRadius - minRadius) / (waypointCount - 1));

                float height = -i * _heightStep;

                waypoints[i] =
                    player.position +
                    _playerOffset +
                    new Vector3(Mathf.Cos(angle) * radius, height, Mathf.Sin(angle) * radius
                    );
            }

            waypoints[^1] = player.position + _playerOffset;

            return waypoints;
        }
    }
}