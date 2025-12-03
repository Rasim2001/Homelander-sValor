using System;
using DG.Tweening;
using UnityEngine;

namespace Units
{
    public abstract class StuckInGround<T> : MonoBehaviour where T : MonoBehaviour
    {
        [SerializeField] private ObserverTrigger _observerTrigger;

        [SerializeField] private float duration = 1;
        [SerializeField] private Vector3 strength = new Vector3(3, 1, 7);
        [SerializeField] private int vibrato = 10;
        [SerializeField] private float randomness = 10;

        protected Tweener ShakeTween;
        protected Coroutine DestroyCoroutine;

        private T _arrow;
        private bool _isTriggered;

        private void Awake() =>
            _arrow = GetComponent<T>();

        private void Start() =>
            _observerTrigger.OnTriggerEnter += StopArrow;

        private void OnDestroy() =>
            _observerTrigger.OnTriggerEnter -= StopArrow;

        protected void OnDisable()
        {
            _isTriggered = false;

            if (DestroyCoroutine != null)
            {
                StopCoroutine(DestroyCoroutine);
                DestroyCoroutine = null;
            }
        }

        private void StopArrow()
        {
            if (_isTriggered || !gameObject.activeInHierarchy)
                return;

            _isTriggered = true;

            ShakeTween = transform.DOShakeRotation(
                duration: duration,
                strength: strength,
                vibrato: vibrato,
                randomness: randomness);

            OnGroundHitLogic(_arrow);
        }

        protected abstract void OnGroundHitLogic(T arrow);
    }
}