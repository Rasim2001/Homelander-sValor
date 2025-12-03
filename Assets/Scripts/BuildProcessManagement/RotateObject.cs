using UnityEngine;

namespace BuildProcessManagement
{
    public class RotateObject : MonoBehaviour
    {
        [SerializeField] private float _rotationSpeed;
        [SerializeField] private Rigidbody2D _rb;

        private void Update()
        {
            if (_rb.velocity.y >= 0)
                return;

            int directionAngle = _rb.velocity.x > 0 ? 1 : -1;
            float angle = Mathf.Atan2(_rb.velocity.y, _rb.velocity.x) * Mathf.Rad2Deg * directionAngle;

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(new Vector3(0, 0, angle)),
                _rotationSpeed * Time.deltaTime);
        }
    }
}