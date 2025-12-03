using Player;
using UnityEngine;

namespace Units
{
    public class UnitFlip : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;

        public void SetFlip(float distance, int direction, PlayerFlip playerFlip)
        {
            if (distance < 0.1f)
                _spriteRenderer.flipX = playerFlip.FlipBoolValue();
            else
                _spriteRenderer.flipX = direction != 1;
        }

        public void SetFlip(float distanceDirectionToMove) =>
            _spriteRenderer.flipX = distanceDirectionToMove < 0;

        public void SetFlip(bool value) =>
            _spriteRenderer.flipX = value;

        public void SetFlip(int value) =>
            _spriteRenderer.flipX = value == 1;
    }
}