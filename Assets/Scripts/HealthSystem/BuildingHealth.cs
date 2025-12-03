using BuildProcessManagement;
using MinimapCore;
using Player.Orders;
using UnityEngine;
using Zenject;

namespace HealthSystem
{
    public class BuildingHealth : MonoBehaviour, IHealth
    {
        [SerializeField] private Minimap _minimap;
        [SerializeField] private OrderMarker _orderMarker;
        [SerializeField] private BuildInfo _buildInfo;
        [SerializeField] private HealingProgress _healingProgress;
        [SerializeField] private DestructionProgress _destructionProgress;

        private IDestroyCommandExecutor _destroyCommandExecutor;

        [field: SerializeField] public int CurrentHP { get; set; }
        [field: SerializeField] public int MaxHp { get; set; }
        public bool IsDeath { get; set; }


        [Inject]
        public void Construct(IDestroyCommandExecutor destroyCommandExecutor) =>
            _destroyCommandExecutor = destroyCommandExecutor;

        public void Initialize(int hp)
        {
            CurrentHP = hp;
            MaxHp = hp;
        }

        public void Reset() =>
            CurrentHP = MaxHp;

        public void TakeDamage(int damage)
        {
            if (_orderMarker.OrderID == OrderID.Build)
                _orderMarker.OrderID = OrderID.Heal;

            CurrentHP -= damage;

            float healthRatio = (float)CurrentHP / MaxHp;
            _destructionProgress.UpdateDestructionProgress(healthRatio);

            if (CurrentHP < 0)
            {
                IsDeath = true;
                _destroyCommandExecutor.DestroyBuild(_buildInfo);
            }

            else
                _minimap.ShowHit();
        }

        public void HealBuilding(int hp)
        {
            if (CurrentHP >= MaxHp)
                return;

            CurrentHP += hp;

            float healthRatio = (float)CurrentHP / MaxHp;

            _healingProgress.UpdateHealProgress(healthRatio);
        }
    }
}