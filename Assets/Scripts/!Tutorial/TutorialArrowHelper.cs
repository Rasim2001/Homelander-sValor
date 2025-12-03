using UnityEngine;

namespace _Tutorial
{
    public class TutorialArrowHelper : MonoBehaviour
    {
        [SerializeField] private Transform _playerTransform;

        [SerializeField] private GameObject _leftArrow;
        [SerializeField] private GameObject _rightArrow;

        private Transform _target;

        private bool _lastArrowValue;
        private bool _switchOnValue;

        public void SetTarget(Transform target) =>
            _target = target;

        public void Update()
        {
            if (_target == null)
                return;

            if (Mathf.Abs(_target.position.x - _playerTransform.position.x) > 10)
            {
                bool isRightArrow = _target.transform.position.x > _playerTransform.position.x;
                SetActive(isRightArrow);
            }
            else
                SwitchOff();
        }

        private void SetActive(bool rightValue)
        {
            _switchOnValue = false;
            _rightArrow.SetActive(rightValue);
            _leftArrow.SetActive(!rightValue);
        }

        private void SwitchOff()
        {
            if (_switchOnValue)
                return;

            _rightArrow.SetActive(false);
            _leftArrow.SetActive(false);

            _switchOnValue = true;
        }
    }
}