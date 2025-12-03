namespace Units.Animators
{
    public class ShielderAnimator : UnitAnimator, IAttackAnimator
    {
        public void PlayAttackAnimation(bool value) =>
            Animator.SetBool(IsAttackingHash, value);

        public void SetRetreatAnimation(bool value) =>
            Animator.SetBool(IsRetreatingHash, value);
    }
}