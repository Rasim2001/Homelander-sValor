using UnityEngine;

namespace Enemy
{
    public class AnimateAlongEnemy : MonoBehaviour
    {
        [SerializeField] private EnemyAnimator _animator;
        [SerializeField] private EnemyMove _move;

        private void LateUpdate() =>
            _animator.PlayMoveAnimation(_move.IsActiveMove);
    }
}