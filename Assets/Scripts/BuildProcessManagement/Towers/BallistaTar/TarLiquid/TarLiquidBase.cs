using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Enemy.Effects;
using Units;
using UnityEngine;

namespace BuildProcessManagement.Towers.BallistaTar
{
    public abstract class TarLiquidBase : MonoBehaviour
    {
        [SerializeField] private UnitObserverTrigger _unitObserverTrigger;

        private readonly List<Collider2D> _affectedEnemies = new List<Collider2D>();

        private void Start()
        {
            TriggerEnter();

            _unitObserverTrigger.OnTriggerEnter += TriggerEnter;

            StartCoroutine(StartDestroy());
        }

        private void OnDestroy() =>
            _unitObserverTrigger.OnTriggerEnter -= TriggerEnter;

        private IEnumerator StartDestroy()
        {
            yield return new WaitForSeconds(3);

            Destroy(gameObject);
        }

        private void TriggerEnter()
        {
            List<Collider2D> enemies = _unitObserverTrigger.GetAllHits();

            List<Collider2D> newEnemies = enemies.Where(x => !_affectedEnemies.Contains(x)).ToList();
            _affectedEnemies.AddRange(newEnemies);

            foreach (Collider2D newEnemy in newEnemies)
            {
                IEnemyEffectSystem effectSystem = newEnemy.GetComponentInParent<IEnemyEffectSystem>();
                AddAllEffects(effectSystem);
            }
        }

        protected abstract void AddAllEffects(IEnemyEffectSystem effectSystem);
    }
}