using HealthSystem;
using Player.Orders;
using UnityEngine;

namespace Test
{
    public class InputTest : MonoBehaviour
    {
        private int _layerMask;

        private void Start() =>
            _layerMask = 1 << LayerMask.NameToLayer("BuildingDamage");

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit2D hit = CastRaycast();
                if (hit.collider != null)
                {
                    if (hit.collider.TryGetComponent(out BuildingHealth health))
                    {
                        if (!hit.collider.GetComponentInParent<OrderMarker>().IsStarted)
                            health.TakeDamage(40);
                    }
                }
            }
        }


        private RaycastHit2D CastRaycast()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit2D = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, _layerMask);
            return hit2D;
        }
    }
}