using UnityEngine;

namespace DayCycle
{
    public class MoonRotation : MonoBehaviour
    {
        [SerializeField] private float _angle;

        private void Update() =>
            transform.rotation = Quaternion.Euler(0, 0, _angle);
    }
}