namespace Units.Animators
{
    public class ArcherAnimator : UnitAnimator, IAttackAnimator
    {
        public void PlayAttackAnimation(bool value) =>
            Animator.SetBool(IsAttackingHash, value);
    }
}