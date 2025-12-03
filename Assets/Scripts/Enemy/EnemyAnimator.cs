using System.Linq;
using UnityEngine;

namespace Enemy
{
    public class EnemyAnimator : MonoBehaviour
    {
        private static readonly int IsMove = Animator.StringToHash("IsMove");
        private static readonly int Attack = Animator.StringToHash("Attack");
        private static readonly int IsDeath = Animator.StringToHash("IsDeath");
        private static readonly int IsFlagDestroyed = Animator.StringToHash("IsFlagDestroyed");
        private static readonly int RunAwayHash = Animator.StringToHash("RunAway");
        private static readonly int IsStunedHash = Animator.StringToHash("IsStuned");

        [SerializeField] private Animator _animator;

        public void PlayMoveAnimation(bool value) =>
            _animator.SetBool(IsMove, value);

        public void PlayAttackAnimation() =>
            _animator.SetTrigger(Attack);

        public void PlayRunAway(bool value) =>
            _animator.SetBool(RunAwayHash, value);

        public void PlayDeathAnimation()
        {
            _animator.StopPlayback();
            _animator.SetTrigger(IsDeath);
        }

        public void PlayFlagDestroyedAnimation() =>
            _animator.SetBool(IsFlagDestroyed, true);

        public void SetAnimatorSpeed(float value) =>
            _animator.speed = value;

        public float GetAnimatorSpeed() =>
            _animator.speed;

        public void PlayStunEffectAnimation(bool value) =>
            _animator.SetBool(IsStunedHash, value);

        public float GetDeathAnimationClipLength()
        {
            AnimationClip animationClip =
                _animator.runtimeAnimatorController.animationClips.First(x => x.name == "Death");

            return animationClip != null ? animationClip.length : 0;
        }
    }
}