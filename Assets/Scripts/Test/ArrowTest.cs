using DG.Tweening;
using Units.RangeUnits;
using UnityEngine;

namespace Test
{
    public class ArrowTest : MonoBehaviour
    {
        [SerializeField] private ObserverTrigger _observerTrigger;

        [SerializeField] private float duration = 1f;
        [SerializeField] private Vector3 strength = new Vector3(0, 0, 15f);
        [SerializeField] private int vibrato = 10;
        [SerializeField] private float randomness = 90;
        [SerializeField] private bool fadeOut = true;
        [SerializeField] private Ease ease = Ease.Linear;

        private Rigidbody2D _rigidbody;
        private ArrowRotation _arrowRotation;

        private void Awake()
        {
            _arrowRotation = GetComponent<ArrowRotation>();
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            _observerTrigger.OnTriggerEnter += StopArrow;
        }

        private void OnDestroy()
        {
            _observerTrigger.OnTriggerEnter -= StopArrow;
        }

        private void StopArrow()
        {
            _arrowRotation.enabled = false;

            _rigidbody.gravityScale = 0;
            _rigidbody.velocity = Vector2.zero;

            transform.DOShakeRotation(
                duration: duration,
                strength: strength,
                vibrato: vibrato,
                randomness: randomness
            );

            Debug.Log("StopArrow");
        }
    }
}