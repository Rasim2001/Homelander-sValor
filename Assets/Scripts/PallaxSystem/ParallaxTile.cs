using UnityEngine;

namespace PallaxSystem
{
    public class ParallaxTile : MonoBehaviour
    {
        [SerializeField] private float _multiplier;

        private Transform _cameraTransform;
        private SpriteRenderer _spriteRenderer;

        private Vector3 _lastCameraPosition;
        private float _textureUnitSizeX;

        private void Awake()
        {
            _cameraTransform = Camera.main.transform;
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            _lastCameraPosition = _cameraTransform.position;
            _textureUnitSizeX = GetTextureSize();
        }

        private void LateUpdate()
        {
            Vector3 deltaMovement = _cameraTransform.position - _lastCameraPosition;
            _lastCameraPosition = _cameraTransform.position;

            float deltaX = _lastCameraPosition.x - transform.position.x;
            if (Mathf.Abs(deltaX) >= _textureUnitSizeX)
            {
                float offset = deltaX % _textureUnitSizeX;
                transform.position = new Vector2(_lastCameraPosition.x + offset, transform.position.y);
            }
            else
                transform.position += new Vector3(deltaMovement.x * _multiplier, deltaMovement.y);
        }

        private float GetTextureSize()
        {
            Sprite sprite = _spriteRenderer.sprite;
            Texture2D texture2D = sprite.texture;

            return texture2D.width / sprite.pixelsPerUnit;
        }
    }
}