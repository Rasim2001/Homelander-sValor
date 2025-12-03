using System;
using UnityEngine;

namespace Test
{
    public class TestShoot : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Transform _shootPoint;
        [SerializeField] private GameObject _arrowPrefab;
        [SerializeField] private float _angleInDegrees;
        [SerializeField] private float _gravity;

        [Header("Other")]
        [SerializeField] private Transform _target;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                Shoot(_target);
        }

        public void Shoot(Transform target)
        {
            if (target == null)
                return;

            Vector3 correctShootPosition = SetCorrectShootPosition();

            float deltaPivotY = 0.8f;

            float x = (target.position - correctShootPosition).x; //TODO:
            float y = (target.position - correctShootPosition).y + deltaPivotY;

            float angleDirection = x > 0 ? _angleInDegrees : 180 - _angleInDegrees;
            float angleInRadians = angleDirection * Mathf.Deg2Rad;

            float v2 = _gravity * x * x /
                       (2 * (y - Mathf.Tan(angleInRadians) * x) * Mathf.Pow(Mathf.Cos(angleInRadians), 2));

            float v = Mathf.Sqrt(Math.Abs(v2));

            float horizontalVelocity = Mathf.Cos(angleInRadians) * v;
            float verticalVelocity = Mathf.Sin(angleInRadians) * v;

            GameObject arrow = Instantiate(_arrowPrefab, correctShootPosition, Quaternion.identity);
            Rigidbody2D arrowRigidbody = arrow.GetComponent<Rigidbody2D>();
            arrowRigidbody.gravityScale = _gravity / 9.81f;
            arrowRigidbody.velocity = new Vector2(horizontalVelocity, verticalVelocity);
        }

        private Vector3 SetCorrectShootPosition()
        {
            int signFactor = -1;

            float positionX = _spriteRenderer.flipX
                ? _shootPoint.position.x
                : _shootPoint.position.x * signFactor;

            return new Vector3(positionX, _shootPoint.position.y);
        }
    }
}