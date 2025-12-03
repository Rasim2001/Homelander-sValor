using UnityEngine;

namespace BuildProcessManagement.ResourceElements
{
    public class TreeResourceAnimator : MonoBehaviour
    {
        private static readonly int IsBreak = Animator.StringToHash("IsBreak");

        [SerializeField] private Animator _animator;

        public void PlayBreakTreeAnimation() =>
            _animator.SetTrigger(IsBreak);
    }
}