using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace MinimapCore
{
    public class BarricadeDestroyedMinimap : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;

        private void Awake() =>
            _spriteRenderer = GetComponent<SpriteRenderer>();

        public async UniTask ShowAsync()
        {
            _spriteRenderer.color = Color.white;

            await _spriteRenderer.DOFade(0f, 2f).SetEase(Ease.OutSine).ToUniTask();
        }
    }
}