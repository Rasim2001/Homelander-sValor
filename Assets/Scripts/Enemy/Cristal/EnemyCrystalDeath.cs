using DG.Tweening;
using Infastructure.StaticData.StaticDataService;
using Player;
using UnityEngine;
using Zenject;

namespace Enemy.Cristal
{
    public class EnemyCrystalDeath : MonoBehaviour
    {
        private static readonly int FullDistortionFade = Shader.PropertyToID("_FullDistortionFade");

        [SerializeField] private Health _health;
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private IStaticDataService _staticDataService;
        private Material _deathMaterialInstance;

        [Inject]
        public void Construct(IStaticDataService staticDataService) =>
            _staticDataService = staticDataService;

        private void Awake()
        {
            Material deathMaterial = _staticDataService.DefaultMaterialStaticData.EnemyDeathMaterial;
            _deathMaterialInstance = new Material(deathMaterial);
        }

        private void Start() =>
            _health.OnDeathHappened += Death;

        private void OnDestroy() =>
            _health.OnDeathHappened -= Death;

        private void Death()
        {
            DOTween.To(
                    () => _spriteRenderer.color,
                    x => _spriteRenderer.color = x,
                    Color.black,
                    1
                ).SetEase(Ease.Linear)
                .OnComplete(DeathAfterAnimation);
        }


        public void DeathAfterAnimation()
        {
            _spriteRenderer.material = _deathMaterialInstance;
            float time = 1;
            float distortionFadeValue = 1;

            DOTween.To(
                    () => distortionFadeValue,
                    x =>
                    {
                        distortionFadeValue = x;
                        _deathMaterialInstance.SetFloat(FullDistortionFade, distortionFadeValue);
                    },
                    0,
                    time
                )
                .SetEase(Ease.Linear)
                .OnComplete(DeathAfterEffects);
        }


        private void DeathAfterEffects() =>
            Destroy(gameObject);
    }
}