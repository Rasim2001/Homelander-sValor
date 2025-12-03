using Units.UnitStates;
using UnityEngine;

namespace BuildProcessManagement.Towers.BallistaBow
{
    [RequireComponent(typeof(Animator))]
    public class BallistaBowAnimator : MonoBehaviour, IBallistaBowAnimator
    {
        private Animator _animator;

        private void Awake() =>
            _animator = GetComponent<Animator>();

        public void Shoot()
        {
            _animator.ResetTrigger(UnitStatesPath.ReloadHash);

            _animator.SetTrigger(UnitStatesPath.ShootHash);
        }

        public void Reload()
        {
            _animator.ResetTrigger(UnitStatesPath.ShootHash);

            _animator.SetTrigger(UnitStatesPath.ReloadHash);
        }
    }
}