using UnityEngine;

namespace Bonfire.Builds
{
    public class ChestBonfire : MonoBehaviour, ISetParent
    {
        private static readonly int IsShowedHash = Animator.StringToHash("IsShowed");

        [SerializeField] private Animator _animator;

        public void SetParent(Transform parent) =>
            transform.SetParent(parent);

        public void Show() =>
            _animator.SetBool(IsShowedHash, true);

        public void Hide() =>
            _animator.SetBool(IsShowedHash, false);
    }
}