using UnityEngine;

namespace Infastructure.Services.Fence
{
    public abstract class FenceMarker : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;

        [SerializeField] private Vector2 _defaultSize;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();

            _defaultSize = _spriteRenderer.size;
        }


        public void SetWidth(float valueX) =>
            _spriteRenderer.size = new Vector2(valueX, _spriteRenderer.size.y);

        public void SetDefaultSettings() =>
            _spriteRenderer.size = _defaultSize;

        public void SetPosition(float position) =>
            transform.position = new Vector3(position, transform.position.y);
    }
}