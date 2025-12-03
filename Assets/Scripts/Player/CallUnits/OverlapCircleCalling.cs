using Infastructure.Services.UnitRecruiter;
using Units.UnitStatusManagement;
using UnityEngine;

namespace Player.CallUnits
{
    public class OverlapCircleCalling : ICallingUnits
    {
        private const string UnitLayer = "Unit";

        private readonly UnitsRecruiterService _unitsRecruiterService;
        private readonly PlayerFlip _playerFlip;
        private readonly Collider2D[] _units = new Collider2D[5];
        private readonly float _detectionRadius = 1.5f;

        private int _unitLayerMask;

        public OverlapCircleCalling(UnitsRecruiterService unitsRecruiterService, PlayerFlip playerFlip)
        {
            _unitsRecruiterService = unitsRecruiterService;
            _playerFlip = playerFlip;
        }

        public void Initialize() =>
            _unitLayerMask = 1 << LayerMask.NameToLayer(UnitLayer);

        public void DetectUnits()
        {
            _playerFlip.transform.localPosition =
                new Vector3(-_playerFlip.FlipValue() * 1.5f, 1);

            int size = Physics2D.OverlapCircleNonAlloc(_playerFlip.transform.position, _detectionRadius,
                _units,
                _unitLayerMask);

            for (int i = 0; i < size; i++)
            {
                UnitStatus unitStatus = _units[i].GetComponentInParent<UnitStatus>();
                _unitsRecruiterService.AddUnitToList(unitStatus);
            }

            DrawDetectionCircle(_playerFlip.transform.position, _detectionRadius);
        }

        private void DrawDetectionCircle(Vector2 center, float radius, int segments = 50)
        {
            float angleStep = 360f / segments; // Угол между сегментами

            for (int i = 0; i < segments; i++)
            {
                float angle1 = Mathf.Deg2Rad * angleStep * i;
                float angle2 = Mathf.Deg2Rad * angleStep * (i + 1);

                Vector2 point1 = new Vector2(Mathf.Cos(angle1), Mathf.Sin(angle1)) * radius + center;
                Vector2 point2 = new Vector2(Mathf.Cos(angle2), Mathf.Sin(angle2)) * radius + center;

                Debug.DrawLine(point1, point2, Color.green, 1f);
            }
        }
    }
}