using System;
using UnityEngine;

namespace Player
{
    public class PlayerAnimator : MonoBehaviour
    {
        private static readonly int IsBuildingState = Animator.StringToHash("IsBuildingState");
        private static readonly int IsWalking = Animator.StringToHash("IsWalking");
        private static readonly int IsRunning = Animator.StringToHash("IsRunning");
        private static readonly int GiveOrder = Animator.StringToHash("GiveOrder");
        private static readonly int Summon = Animator.StringToHash("Summon");
        private static readonly int Release = Animator.StringToHash("Release");
        private static readonly int Shoot = Animator.StringToHash("Shoot");
        private static readonly int CastSkill = Animator.StringToHash("CastSkill");
        private static readonly int RunTransition = Animator.StringToHash("RunTransition");
        private static readonly int ResetToIdleTriggerHash = Animator.StringToHash("ResetToIdleTrigger");
        private static readonly int IsFlyingHash = Animator.StringToHash("IsFlying");

        private static readonly int AccelerationTime = Animator.StringToHash("AccelerationTime");

        [SerializeField] private Animator _animator;

        public bool AnyStateIsActive;
        public bool TiredIsActive;
        public bool IsBuildingModeActive;


        public void SetSpeed(float value) =>
            _animator.speed = value;

        public float GetSpeed() =>
            _animator.speed;


        public void SetAccelerationTime(float value) =>
            _animator.SetFloat(AccelerationTime, value);

        public void PlayFlyAnimation(bool value) =>
            _animator.SetBool(IsFlyingHash, value);

        public void PlayForceIdleAnimation()
        {
            _animator.SetTrigger(ResetToIdleTriggerHash);

            PlayWalkAnimation(false);
            PlayRunAnimation(false);
        }

        public void ResetForceIdleTrigger() =>
            _animator.ResetTrigger(ResetToIdleTriggerHash);

        public void PlayWalkAnimation(bool value) =>
            _animator.SetBool(IsWalking, value);

        public void PlayBuildModeAnimation(bool value)
        {
            IsBuildingModeActive = value;

            _animator.SetBool(IsBuildingState, value);
        }

        public void PlayRunAnimation(bool value) =>
            _animator.SetBool(IsRunning, value);

        public void PlaySummonAnimation()
        {
            _animator.SetBool(RunTransition, false);
            ExitTiredTR();
            _animator.SetTrigger(Summon);
        }

        public void PlayGiveOrderAnimation()
        {
            _animator.SetBool(RunTransition, false);
            ExitTiredTR();
            _animator.SetTrigger(GiveOrder);
        }

        public void PlayReleaseAnimation()
        {
            _animator.SetBool(RunTransition, false);
            ExitTiredTR();
            _animator.SetTrigger(Release);
        }

        public void PlayCastSkillAnimation()
        {
            _animator.SetBool(RunTransition, false);
            ExitTiredTR();
            _animator.SetTrigger(CastSkill);
        }


        public void PlayAimAndShootAnimation()
        {
            _animator.SetBool(RunTransition, false);
            ExitTiredTR();
            _animator.SetTrigger(Shoot);
        }


        public void StartRunTR() =>
            _animator.SetBool(RunTransition, true);

        public void StopRunTR() =>
            _animator.SetBool(RunTransition, false);

        public void StartTiredTR() =>
            TiredIsActive = true;

        public void ExitTiredTR() =>
            TiredIsActive = false;


        public void StartAnyStateAnimation() =>
            AnyStateIsActive = true;

        public void ExitAnyStateAnimation() =>
            AnyStateIsActive = false;
    }
}