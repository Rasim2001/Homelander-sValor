using UnityEngine;

namespace BuildProcessManagement
{
    public class FlagTooltipPositioner : MonoBehaviour
    {
        [SerializeField] private Transform _flagTransform;

        [SerializeField] private Transform _leftSideTransform;
        [SerializeField] private Transform _rightSideTransform;

        private SpriteRenderer _spriteRenderer;

        private void Awake() =>
            _spriteRenderer = _flagTransform.GetComponent<SpriteRenderer>();


        private void Start()
        {
            _flagTransform.position =
                transform.position.x > 0 ? _leftSideTransform.position : _rightSideTransform.position;

            _spriteRenderer.flipX = transform.position.x > 0;
        }
    }
}