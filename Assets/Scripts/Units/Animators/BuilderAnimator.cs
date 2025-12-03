namespace Units.Animators
{
    public class BuilderAnimator : UnitAnimator
    {
        public void SetWorkingStateAnimation(bool value) =>
            Animator.SetBool(IsWorkingStateHash, value);

        public void SetWorkingStateSuccessfullyСompleted() =>
            Animator.SetTrigger(IsWorkingStateSuccessfullyСompletedHash);

        public override void ResetAllAnimations()
        {
            base.ResetAllAnimations();

            SetWorkingStateAnimation(false);
        }
    }
}