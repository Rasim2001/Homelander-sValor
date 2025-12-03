using System;
using UnityEngine;

namespace Units.Animators
{
    public class UnitAnimator : MonoBehaviour
    {
        private static readonly int IsIdlingHash = Animator.StringToHash("IsIdling");
        private static readonly int IsWalkingHash = Animator.StringToHash("IsWalking");
        private static readonly int IsRunningHash = Animator.StringToHash("IsRunning");
        private static readonly int IsFastRunningHash = Animator.StringToHash("IsFastRunning");

        protected static readonly int IsWorkingStateHash = Animator.StringToHash("IsWorkingState");
        protected static readonly int IsWorkingStateSuccessfullyСompletedHash =
            Animator.StringToHash("IsWorkingStateSuccessfullyСompleted");

        protected static readonly int IsAttackingHash = Animator.StringToHash("IsAttacking");
        protected static readonly int IsRetreatingHash = Animator.StringToHash("IsRetreating");

        [SerializeField] protected Animator Animator;


        public void ChangeAnimationSpeed(float value) =>
            Animator.speed = value;


        public bool IsStandartAnimationSpeed() =>
            Math.Abs(Animator.speed - 1) < 0.01f;

        public void SetIdleAnimation(bool value) =>
            Animator.SetBool(IsIdlingHash, value);

        public void SetWalkAnimation(bool value) =>
            Animator.SetBool(IsWalkingHash, value);

        public void SetRunAnimation(bool value) =>
            Animator.SetBool(IsRunningHash, value);

        public void SetFastRunAnimation(bool value) =>
            Animator.SetBool(IsFastRunningHash, value);


        public virtual void ResetAllAnimations()
        {
            SetIdleAnimation(false);
            SetWalkAnimation(false);
            SetRunAnimation(false);
            SetFastRunAnimation(false);
        }
    }
}