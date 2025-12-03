using System.Collections;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BuildProcessManagement.ResourceElements
{
    public class TreeDestroyResource : DestroyResource
    {
        [SerializeField] private TreeResourceAnimator _treeResourceElements;
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private void Start() =>
            _spriteRenderer.flipX = Random.Range(0, 2) == 0;


        public override void DestroyElement()
        {
            base.DestroyElement();

            _treeResourceElements.PlayBreakTreeAnimation();

            StartCoroutine(DestroyFromWorldCoroutine());
        }

        private IEnumerator DestroyFromWorldCoroutine()
        {
            yield return new WaitForSeconds(2);

            DOTween.To(
                    () => _spriteRenderer.color,
                    x => _spriteRenderer.color = x,
                    new Color(0, 0, 0, 0), 1f)
                .OnComplete(() => Destroy(gameObject));
        }
    }
}