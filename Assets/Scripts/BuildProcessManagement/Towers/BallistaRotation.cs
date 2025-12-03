using System;
using UnityEngine;

namespace BuildProcessManagement.Towers
{
    public class BallistaRotation : MonoBehaviour, IBallistaRotation
    {
        public event Action OnTargetDeath;

        private Collider2D _target;

        private bool _isDefaultRotation;
        private bool _targetLocked;
        private float _deltaAngle;

        public void Initialize(int deltaAngle)
        {
            _deltaAngle = deltaAngle;

            Vector3 localScale = deltaAngle == 90 ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);
            transform.localScale = localScale;
            transform.localPosition = new Vector3(transform.localPosition.x * localScale.x, transform.localPosition.y);
        }

        public void SetTarget(Collider2D target)
        {
            _target = target;

            if (_target != null)
                _isDefaultRotation = false;
        }

        private void Update()
        {
            if (_targetLocked)
                return;

            if (_target == null)
            {
                if (!_isDefaultRotation)
                    FollowToDefaultRotation();
            }
            else
                FollowRotation();
        }


        public bool HasReachedTargetRotation()
        {
            if (_target == null)
                OnTargetDeath?.Invoke();

            Vector3 targetOffset = new Vector3(0, 0.3f, 0);

            Vector3 direction = _target.transform.position + targetOffset - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90;

            Quaternion targetRotation = Quaternion.Euler(0, 0, angle + _deltaAngle);

            return Quaternion.Angle(transform.rotation, targetRotation) <= 1;
        }

        public void SetTargetLock() =>
            _targetLocked = true;

        public void UnlockTarget() =>
            _targetLocked = false;


        private void FollowRotation()
        {
            Vector3 targetOffset = new Vector3(0, 0.3f, 0);

            Vector3 direction = _target.transform.position + targetOffset - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90f;

            Quaternion targetRotation = Quaternion.Euler(0, 0, angle + _deltaAngle);
            transform.rotation =
                Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10);
        }

        private void FollowToDefaultRotation()
        {
            transform.rotation =
                Quaternion.Lerp(transform.rotation, Quaternion.identity, Time.deltaTime * 10);

            if (transform.rotation.z == 0)
                _isDefaultRotation = true;
        }
    }
}