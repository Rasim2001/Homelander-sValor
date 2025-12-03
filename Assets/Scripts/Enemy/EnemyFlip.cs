using UnityEngine;

namespace Enemy
{
    public class EnemyFlip : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;

        public void SetFlip(bool value) =>
            _spriteRenderer.flipX = value;
        public void SetFlip(float distanceDirectionToMove) =>
            _spriteRenderer.flipX = distanceDirectionToMove < 0;
    }
}