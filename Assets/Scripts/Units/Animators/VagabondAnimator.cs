using UnityEngine;

namespace Units.Animators
{
    public class VagabondAnimator : UnitAnimator
    {
        private static readonly int IsFearingHash = Animator.StringToHash("IsFearing");
        private static readonly int IsScaryRunningHash = Animator.StringToHash("IsScaryRunning");

        public void PlayFearAnimation(bool value) =>
            Animator.SetBool(IsFearingHash, value);

        public void PlayScaryRunAnimation(bool value) =>
            Animator.SetBool(IsScaryRunningHash, value);
    }
}