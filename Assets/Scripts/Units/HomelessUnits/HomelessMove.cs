using System;
using Random = UnityEngine.Random;

namespace Units.HomelessUnits
{
    public class HomelessMove : UnitMove
    {
        private float _savedPositionX;

        public void ChangeTargetPosition(float value) =>
            _savedPositionX = value;

        public override void ChangeTargetPosition()
        {
            float leftBorder = _savedPositionX - 5;
            float rightBorder = _savedPositionX + 5;

            TargetFreePositionX = FindPrefferedTarget(leftBorder, rightBorder);
        }

        public void ChangeTargetHomelessPosition()
        {
            float randomDistance = Random.Range(20, 30);

            float prefferedPointX = FlagTrackerService.GetLastFlag(transform.position.x > 0).position.x;
            int directionSign = Math.Sign(transform.position.x);

            _savedPositionX = prefferedPointX + randomDistance * directionSign;
            TargetFreePositionX = _savedPositionX;
        }
    }
}