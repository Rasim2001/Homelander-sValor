using System;
using UnityEngine;

namespace Units.RangeUnits
{
    public abstract class ItemShooterBase : MonoBehaviour
    {
        public Transform ShootPoint;
        public float AngleInDegrees;
        public float Gravity;

        public void Shoot(Transform target)
        {
            if (target == null)
                return;

            float deltaPivotY = 0.8f;

            float x = (target.position - ShootPoint.position).x;
            float y = (target.position - ShootPoint.position).y + deltaPivotY;

            float angleDirection = x > 0 ? AngleInDegrees : 180 - AngleInDegrees;
            float angleInRadians = angleDirection * Mathf.Deg2Rad;

            float v2 = Gravity * x * x /
                       (2 * (y - Mathf.Tan(angleInRadians) * x) * Mathf.Pow(Mathf.Cos(angleInRadians), 2));

            float v = Mathf.Sqrt(Math.Abs(v2));

            float horizontalVelocity = Mathf.Cos(angleInRadians) * v;
            float verticalVelocity = Mathf.Sin(angleInRadians) * v;

            CreateBullet(horizontalVelocity, verticalVelocity, Gravity);
        }

        public void Shoot(Vector3 targetPosition)
        {
            float deltaPivotY = 0.8f;

            float x = (targetPosition - ShootPoint.position).x;
            float y = (targetPosition - ShootPoint.position).y + deltaPivotY;

            float angleDirection = x > 0 ? AngleInDegrees : 180 - AngleInDegrees;
            float angleInRadians = angleDirection * Mathf.Deg2Rad;

            float v2 = Gravity * x * x /
                       (2 * (y - Mathf.Tan(angleInRadians) * x) * Mathf.Pow(Mathf.Cos(angleInRadians), 2));

            float v = Mathf.Sqrt(Math.Abs(v2));

            float horizontalVelocity = Mathf.Cos(angleInRadians) * v;
            float verticalVelocity = Mathf.Sin(angleInRadians) * v;

            CreateBullet(horizontalVelocity, verticalVelocity, Gravity);
        }


        protected abstract void CreateBullet(float horizontalVelocity, float verticalVelocity, float gravity);
    }
}