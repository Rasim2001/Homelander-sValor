using UnityEngine;

namespace BuildProcessManagement.Towers.BallistaTar
{
    public class Jug : MonoBehaviour
    {
        private static readonly int JugHash = Animator.StringToHash("Jug");
        private static readonly int DefaultHash = Animator.StringToHash("Default");

        private const int SortingLayerJug = 6;
        private const int DefaultLayerJug = 2;

        [SerializeField] private SpriteRenderer _jugSpriteRender;
        [SerializeField] private Animator _animator;

        public void SetDefaultLayer() =>
            _jugSpriteRender.sortingOrder = DefaultLayerJug;

        public void OverrideSpriteLayer() =>
            _jugSpriteRender.sortingOrder = SortingLayerJug;

        public void PlayAnimation()
        {
            _animator.Play(JugHash, 0, 0);
            _jugSpriteRender.enabled = true;
        }

        public void ExitAnimation()
        {
            _animator.Play(DefaultHash, 0, 0);
            _jugSpriteRender.enabled = false;

            SetDefaultLayer();
        }
    }
}