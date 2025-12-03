using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Enemy.Effects.ArmorBreak;
using Enemy.Effects.Bleed;
using Enemy.Effects.Convert;
using Enemy.Effects.Damage;
using Enemy.Effects.EffectDisplayUI;
using Enemy.Effects.Fear;
using Enemy.Effects.Freez;
using Enemy.Effects.Knockback;
using Enemy.Effects.Miss;
using Enemy.Effects.Slowdown;
using Enemy.Effects.Stun;
using Infastructure;
using Infastructure.Services.AssetProvider;
using Infastructure.StaticData.Enemy;
using Infastructure.StaticData.StaticDataService;
using Player;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;
using Type = System.Type;

namespace Enemy.Effects
{
    public class EnemyEffectSystem : MonoBehaviour, IEnemyEffectSystem
    {
        [SerializeField] private EnemyInfo _enemyInfo;
        [SerializeField] private Health _health;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private EnemyAttack _enemyAttack;
        [SerializeField] private EnemyMove _enemyMove;
        [SerializeField] private Animator _animator;
        [SerializeField] private EnemyFlip _enemyFlip;
        [SerializeField] private EnemyAnimator _enemyAnimator;
        [SerializeField] private EnemyObserverTrigger _enemyObserverTrigger;
        [SerializeField] private EnemyAgressionZone _agressionZone;
        [SerializeField] private Rigidbody2D _rigidbody;

        [Header("Materials")]
        [SerializeField] private Material _freezMaterial;
        [SerializeField] private Material _convertMaterial;

        [Header("UI Effects")]
        [SerializeField] private Transform _effectsParentTransform;

        private readonly Dictionary<Type, IEnemyEffect> _allEffects = new Dictionary<Type, IEnemyEffect>();
        private readonly Dictionary<IEnemyEffect, Coroutine> _effectDurations =
            new Dictionary<IEnemyEffect, Coroutine>();

        private readonly List<IEnemyEffect> _activeEffects = new List<IEnemyEffect>();
        private readonly List<IEnemyEffect> _removedOverridableEffects = new List<IEnemyEffect>();
        private readonly List<IEffectUpdater> _activeUpdatedEffects = new List<IEffectUpdater>();

        private EffectDisplaySystemUI _effectDisplay;

        private IEnemyEffect _freezEffect;
        private IEnemyEffect _fearEffect;
        private IEnemyEffect _convertEffect;
        private IEnemyEffect _slowdownEffect;
        private IEnemyEffect _missEffect;
        private IEnemyEffect _stunEffect;
        private IEnemyEffect _knockbackEffect;

        private IEnemyEffect _fireEffect;
        private IEnemyEffect _armorBreakEffect;
        private IEnemyEffect _bleedEffect;

        private IStaticDataService _staticDataService;
        private IAssetProviderService _assetProviderService;
        private ICoroutineRunner _coroutineRunner;

        [Inject]
        public void Construct(IStaticDataService staticDataService, IAssetProviderService assetProviderService,
            ICoroutineRunner coroutineRunner)
        {
            _staticDataService = staticDataService;
            _assetProviderService = assetProviderService;
            _coroutineRunner = coroutineRunner;
        }

        private void Awake()
        {
            InitializeUI();
            Initialize();
            SubscribeUpdates();
        }

        private void InitializeUI()
        {
            _effectDisplay = new EffectDisplaySystemUI(_staticDataService, _assetProviderService);
            _effectDisplay.Initialize(_effectsParentTransform);
            _effectDisplay.HideAll();
        }

        private void OnDestroy() =>
            CleanUp();

        private void Update()
        {
            foreach (IEffectUpdater updatedEffect in _activeUpdatedEffects)
                updatedEffect.Update();

            if (_effectDisplay != null)
                _effectDisplay.UpdateProgressBarUI();
        }


        private void Initialize()
        {
            EnemyStaticData enemyStaticData = _staticDataService.ForEnemy(_enemyInfo.EnemyTypeId);

            _freezEffect = new FreezEffect(_enemyAttack, _enemyMove, _animator, _spriteRenderer, _freezMaterial);
            _fearEffect = new FearEffect(_enemyMove, _enemyFlip, _enemyAttack, _enemyAnimator);

            _convertEffect = new ConvertEffect(
                _enemyObserverTrigger,
                _health,
                _agressionZone,
                _enemyMove,
                _enemyFlip,
                _enemyAnimator,
                _enemyAttack,
                _convertMaterial,
                _spriteRenderer);

            _slowdownEffect = new SlowdownEffect(_enemyMove, _enemyAnimator, enemyStaticData);
            _missEffect = new MissEffect(_enemyAttack);
            _stunEffect = new StunEffect(_enemyMove, _enemyAttack, _enemyAnimator);
            _knockbackEffect = new KnockbackEffect(_enemyMove, _enemyAttack, _enemyAnimator, _rigidbody);

            _fireEffect = new FireEffect(_coroutineRunner, _health);
            _armorBreakEffect = new ArmorBreakEffect(_health);
            _bleedEffect = new BleedEffect(_coroutineRunner, _enemyMove, _health);

            _allEffects.Add(typeof(FreezEffect), _freezEffect);
            _allEffects.Add(typeof(FearEffect), _fearEffect);
            _allEffects.Add(typeof(ConvertEffect), _convertEffect);
            _allEffects.Add(typeof(SlowdownEffect), _slowdownEffect);
            _allEffects.Add(typeof(MissEffect), _missEffect);
            _allEffects.Add(typeof(StunEffect), _stunEffect);
            _allEffects.Add(typeof(FireEffect), _fireEffect);
            _allEffects.Add(typeof(ArmorBreakEffect), _armorBreakEffect);
            _allEffects.Add(typeof(BleedEffect), _bleedEffect);
            _allEffects.Add(typeof(KnockbackEffect), _knockbackEffect);
        }

        public void AddEffect<T>(float duration = 100) where T : IEnemyEffect
        {
            Type type = typeof(T);

            if (!_allEffects.TryGetValue(type, out IEnemyEffect enemyEffect) || _health.IsDeath)
                return;

            if (enemyEffect.GetUnaffectedUnits()?.Any(enemyType => enemyType == _enemyInfo.EnemyTypeId) == true)
                return;

            if (_activeEffects.Contains(enemyEffect))
                RemoveLastDurationCoroutine(enemyEffect);
            else
                RegisterEffect(enemyEffect, type, duration);

            AddNewDurationCoroutine<T>(enemyEffect, duration);
        }

        public void AddEffect<T>(Transform sender, float duration = 5) where T : IEnemyEffectWithSender
        {
            Type type = typeof(T);

            if (!_allEffects.TryGetValue(type, out IEnemyEffect enemyEffect) || _health.IsDeath)
                return;

            if (enemyEffect.GetUnaffectedUnits()?.Any(enemyType => enemyType == _enemyInfo.EnemyTypeId) == true)
                return;

            IEnemyEffectWithSender effectSender = enemyEffect as IEnemyEffectWithSender;
            effectSender.SenderTransform = sender;

            RemoveLastDurationCoroutine(enemyEffect);
            RegisterEffectWithSender(enemyEffect);

            AddNewDurationCoroutine<T>(enemyEffect, duration);
        }


        public void RemoveEffect<T>() where T : IEnemyEffect
        {
            Type effectType = typeof(T);

            if (!TryGetActiveEffect(effectType, out IEnemyEffect effect))
                return;

            RemoveFromUpdaters(effect);
            RemoveFromActiveEffects(effect);
            RemoveLastDurationCoroutine(effect);

            if (effect is IEnemyEffectWithSender)
                HandleEffectWithSenderRemoval(effectType, effect);
            else
                HandleRegularEffectRemoval(effectType, effect);
        }

        private bool TryGetActiveEffect(Type effectType, out IEnemyEffect effect) =>
            _allEffects.TryGetValue(effectType, out effect) && _activeEffects.Contains(effect);

        private void RemoveFromUpdaters(IEnemyEffect effect)
        {
            if (effect is IEffectUpdater updater && _activeUpdatedEffects.Contains(updater))
                _activeUpdatedEffects.Remove(updater);
        }

        private void RemoveFromActiveEffects(IEnemyEffect effect) =>
            _activeEffects.Remove(effect);

        private void HandleEffectWithSenderRemoval(Type effectType, IEnemyEffect effect)
        {
            List<IEnemyEffect> localRemovedEffects = GetRemovedOverridableEffects(effectType, effect);

            if (localRemovedEffects == null || localRemovedEffects.Count == 0)
                effect.RemoveEffect();

            HideEffectIfWorked(effectType, effect);

            if (localRemovedEffects != null)
                _removedOverridableEffects.AddRange(localRemovedEffects);

            RestoreAllOverridableEffects();
        }

        private void RestoreAllOverridableEffects()
        {
            foreach (IEnemyEffect activeEffect in _allEffects.Values)
                TryApplyOverridableEffect(activeEffect);
        }

        private void HideEffectIfWorked(Type effectType, IEnemyEffect effect)
        {
            if (effect.IsWorked)
                _effectDisplay.Hide(effectType);
        }


        private void HandleRegularEffectRemoval(Type effectType, IEnemyEffect effect)
        {
            DeactivateEffect(effectType, effect);
            RestoreOverriddenEffects(effect);
            TryApplyOverridableEffect(effect);
        }

        private void DeactivateEffect(Type effectType, IEnemyEffect effect)
        {
            if (effect.IsWorked)
            {
                _effectDisplay.Hide(effectType);
                effect.RemoveEffect();
            }
        }

        private void RestoreOverriddenEffects(IEnemyEffect effect)
        {
            List<Type> overridableEffects = effect.GetOverridableEffects();
            if (overridableEffects == null)
                return;

            List<IEnemyEffect> effectsToRestore = _removedOverridableEffects
                .Where(x => overridableEffects.Contains(x.GetType()))
                .ToList();

            foreach (IEnemyEffect enemyEffect in effectsToRestore)
            {
                _effectDisplay.Hide(enemyEffect.GetType());
                enemyEffect.RemoveEffect();
            }

            foreach (IEnemyEffect restoredEffect in effectsToRestore)
                _removedOverridableEffects.Remove(restoredEffect);
        }

        private List<IEnemyEffect> GetRemovedOverridableEffects(Type effectType, IEnemyEffect effect)
        {
            List<IEnemyEffect> localRemovedEffects = new List<IEnemyEffect>();

            foreach (IEnemyEffect activeEffect in _activeEffects)
            {
                List<Type> types = activeEffect.GetOverridableEffects();

                if (types != null && types.Contains(effectType))
                    localRemovedEffects.Add(effect);
            }

            return localRemovedEffects;
        }

        private void RegisterEffect(IEnemyEffect enemyEffect, Type type, float duration)
        {
            if (enemyEffect is IEffectUpdater updater)
                _activeUpdatedEffects.Add(updater);

            TryOverrideEffect(enemyEffect);

            if (!IsOverriddenByActiveEffects(type))
            {
                _effectDisplay.Show(type);
                _effectDisplay.SetDuration(type, duration);

                enemyEffect.ApplyEffect();
            }

            _activeEffects.Add(enemyEffect);
        }

        private void RegisterEffectWithSender(IEnemyEffect enemyEffect)
        {
            enemyEffect.ApplyEffect();

            if (!_activeEffects.Contains(enemyEffect))
                _activeEffects.Add(enemyEffect);
        }


        private void AddNewDurationCoroutine<T>(IEnemyEffect enemyEffect, float duration) where T : IEnemyEffect
        {
            Coroutine coroutine = StartCoroutine(UpdateDurationCoroutine<T>(enemyEffect, duration));

            _effectDurations.Add(enemyEffect, coroutine);
            _effectDisplay.SetDuration(typeof(T), duration);
        }

        private IEnumerator UpdateDurationCoroutine<T>(IEnemyEffect enemyEffect, float duration) where T : IEnemyEffect
        {
            yield return new WaitForSeconds(duration);

            RemoveLastDurationCoroutine(enemyEffect);
            RemoveEffect<T>();
        }

        private void RemoveLastDurationCoroutine(IEnemyEffect enemyEffect)
        {
            if (_effectDurations.TryGetValue(enemyEffect, out Coroutine existingCoroutine))
            {
                StopCoroutine(existingCoroutine);

                _effectDurations.Remove(enemyEffect);
            }
        }

        private void TryOverrideEffect(IEnemyEffect enemyEffect)
        {
            List<Type> effects = enemyEffect.GetOverridableEffects();

            if (effects == null)
                return;

            foreach (Type effectType in effects)
            {
                if (_allEffects.TryGetValue(effectType, out IEnemyEffect overridableEffect))
                {
                    if (_activeEffects.Contains(overridableEffect) && overridableEffect.IsWorked)
                    {
                        _effectDisplay.Hide(effectType);
                        overridableEffect.RemoveEffect();
                    }
                }
            }
        }

        private void TryApplyOverridableEffect(IEnemyEffect enemyEffect)
        {
            List<Type> effects = enemyEffect.GetOverridableEffects();

            if (effects == null)
                return;

            foreach (Type effectType in effects)
            {
                if (_allEffects.TryGetValue(effectType, out IEnemyEffect overridableEffect))
                {
                    if (_activeEffects.Contains(overridableEffect))
                    {
                        if (overridableEffect is IEnemyEffectWithSender _)
                            continue;

                        if (!IsOverriddenByActiveEffects(effectType))
                        {
                            _effectDisplay.Show(effectType);

                            overridableEffect.ApplyEffect();
                        }
                    }
                }
            }
        }

        private bool IsOverriddenByActiveEffects(Type effectType)
        {
            return _activeEffects.Any(activeEffect =>
                activeEffect.GetOverridableEffects()?.Contains(effectType) == true);
        }


        private void SubscribeUpdates() =>
            _health.OnDeathHappened += RemoveAllEffects;

        private void CleanUp() =>
            _health.OnDeathHappened -= RemoveAllEffects;

        private void RemoveAllEffects()
        {
            foreach (var effectCoroutine in _effectDurations.Values)
            {
                if (effectCoroutine != null)
                    StopCoroutine(effectCoroutine);
            }

            _effectDurations.Clear();

            foreach (IEnemyEffect effect in _activeEffects.ToList())
            {
                if (effect.IsWorked)
                {
                    effect.RemoveEffect();
                    _effectDisplay.Hide(effect.GetType());
                }
            }

            _activeEffects.Clear();
            _activeUpdatedEffects.Clear();

            _effectDisplay.HideAll();
        }


        [ButtonGroup("FreezEffect")]
        public void AddFreezEffect() => AddEffect<FreezEffect>();

        [ButtonGroup("FreezEffect")]
        public void RemoveFreezEffect() => RemoveEffect<FreezEffect>();

        [ButtonGroup("FearEffect")]
        public void AddFearEffect() => AddEffect<FearEffect>();

        [ButtonGroup("FearEffect")]
        public void RemoveFearEffect() => RemoveEffect<FearEffect>();

        [ButtonGroup("ConvertEffect")]
        public void AddConvertEffect() => AddEffect<ConvertEffect>();

        [ButtonGroup("ConvertEffect")]
        public void RemoveConvertEffect() => RemoveEffect<ConvertEffect>();

        [ButtonGroup("SlowdownEffect")]
        public void AddSlowdown() => AddEffect<SlowdownEffect>();

        [ButtonGroup("SlowdownEffect")]
        public void RemoveSlowdown() => RemoveEffect<SlowdownEffect>();

        [ButtonGroup("MissEffect")]
        public void AddMiss() => AddEffect<MissEffect>();

        [ButtonGroup("MissEffect")]
        public void RemoveMiss() => RemoveEffect<MissEffect>();

        [ButtonGroup("StunEffect")]
        public void AddStun() => AddEffect<StunEffect>();

        [ButtonGroup("StunEffect")]
        public void RemoveStun() => RemoveEffect<StunEffect>();

        [ButtonGroup("FireEffect")]
        public void AddFireEffect() => AddEffect<FireEffect>();

        [ButtonGroup("FireEffect")]
        public void RemoveFireEffect() => RemoveEffect<FireEffect>();

        [ButtonGroup("ArmorBreak")]
        public void AddArmorBreakEffect() => AddEffect<ArmorBreakEffect>();

        [ButtonGroup("ArmorBreak")]
        public void RemoveArmorBreakEffect() => RemoveEffect<ArmorBreakEffect>();

        [ButtonGroup("Bleed")]
        public void AddBleedEffect() => AddEffect<BleedEffect>();

        [ButtonGroup("Bleed")]
        public void RemoveBleedEffect() => RemoveEffect<BleedEffect>();

        [ButtonGroup("Knockback")]
        public void AddKnockbackEffect()
        {
            PlayerMove playerMove = FindObjectOfType<PlayerMove>();

            AddEffect<KnockbackEffect>(playerMove.transform, 100);
        }

        [ButtonGroup("Knockback")]
        public void RemoveKnockbackEffect() => RemoveEffect<KnockbackEffect>();
    }
}