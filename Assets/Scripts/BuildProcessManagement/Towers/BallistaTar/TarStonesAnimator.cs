using UnityEngine;

namespace BuildProcessManagement.Towers.BallistaTar
{
    [RequireComponent(typeof(Animator))]
    public class TarStonesAnimator : MonoBehaviour, ITarStonesAnimator
    {
        private readonly int StonesDownHash = Animator.StringToHash("StonesDown");
        private readonly int StonesUpHash = Animator.StringToHash("StonesUp");

        private Animator _animator;

        private void Start() =>
            _animator = GetComponent<Animator>();

        public void PlayStonesDown() =>
            _animator.SetTrigger(StonesDownHash);

        public void PlayStonesUp() =>
            _animator.SetTrigger(StonesUpHash);
    }
}