using UnityEngine;

namespace Units
{
    public class SpeachBubleOrderUpdater : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private void Awake() =>
            DisableSpeachBuble();

        public void UpdateSpeachBuble(Sprite sprite)
        {
            _spriteRenderer.sprite = sprite;
            _spriteRenderer.enabled = true;
        }

        public void DisableSpeachBuble() =>
            _spriteRenderer.enabled = false;
    }
}