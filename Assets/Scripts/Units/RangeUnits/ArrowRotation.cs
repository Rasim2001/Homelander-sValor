using UnityEngine;

namespace Units.RangeUnits
{
    public class ArrowRotation : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D _rb;

        public void Update() =>
            Rotate();

        private void Rotate()
        {
            float angle = Mathf.Atan2(_rb.velocity.y, _rb.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 180);
        }
    }
}