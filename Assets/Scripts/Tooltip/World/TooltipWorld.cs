using TMPro;
using UnityEngine;

namespace Tooltip.World
{
    public class TooltipWorld : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _bgSpriteRenderer;
        [SerializeField] private TextMeshPro _textMeshPro;

        private readonly Vector2 _sizeDelta = new Vector2(0.2f, 0.2f);

        public void SetText(string text)
        {
            _textMeshPro.text = text;

            RegisterSize();
        }

        public Vector3 GetSize()
        {
            _textMeshPro.ForceMeshUpdate();

            Bounds bounds = _textMeshPro.textBounds;
            return bounds.size;
        }

        private void RegisterSize()
        {
            Vector3 size = GetSize();

            _bgSpriteRenderer.size = new Vector3(size.x + _sizeDelta.x, size.y + _sizeDelta.y);
        }
    }
}