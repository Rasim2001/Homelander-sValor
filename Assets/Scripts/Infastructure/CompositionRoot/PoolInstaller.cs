using Bag;
using BuildProcessManagement.Towers;
using BuildProcessManagement.Towers.CatapultTower;
using Enemy.Cristal;
using Infastructure.Services.Fence;
using Infastructure.Services.Pool;
using Infastructure.StaticData.EnemyCristal;
using Loots;
using MinimapCore;
using Player;
using Player.Shoot;
using Sirenix.OdinInspector;
using Tooltip.World;
using Units.RangeUnits;
using UnityEngine;
using Zenject;

namespace Infastructure.CompositionRoot
{
    public class PoolInstaller : MonoInstaller
    {
        [FoldoutGroup("Arrow")] [SerializeField]
        private Transform _arrowContainer;

        [FoldoutGroup("Arrow")] [SerializeField]
        private ArrowDamage _arrowPoolPrefab;

        [FoldoutGroup("CatapultBullet")] [SerializeField]
        private Transform _catapultBulletContainer;

        [FoldoutGroup("CatapultBullet")] [SerializeField]
        private CatapultBullet _catapultBulletPoolPrefab;

        [FoldoutGroup("FearBullet")] [SerializeField]
        private Transform _fearBulletContainer;

        [FoldoutGroup("FearBullet")] [SerializeField]
        private FearBullet _fearBulletPoolPrefab;

        [FoldoutGroup("Tooltip")] [SerializeField]
        private Transform _tooltipWorldContainer;

        [FoldoutGroup("Tooltip")] [SerializeField]
        private TooltipWorld _tooltipWorldPoolPrefab;

        [FoldoutGroup("Fence")] [SerializeField]
        private Transform _fenceContainer;

        [FoldoutGroup("Fence")] [SerializeField]
        private LeftFenceMarker _leftFencePoolPrefab;

        [FoldoutGroup("Fence")] [SerializeField]
        private RightFenceMarker _rightFencePoolPrefab;

        [FoldoutGroup("Coin")] [SerializeField]
        private CoinLoot _coinLootCoinPrefab;

        [FoldoutGroup("Coin")] [SerializeField]
        private Transform _lootCoinContainer;

        [FoldoutGroup("BagOfMoney")] [SerializeField]
        private BagOfMoney _bagPrefab;

        [FoldoutGroup("BagOfMoney")] [SerializeField]
        private Transform _bagContainer;

        [FoldoutGroup("EnemySpawnPoint")] [SerializeField]
        private EnemySpawnPoint _enemySpawnPointPrefab;

        [FoldoutGroup("EnemySpawnPoint")] [SerializeField]
        private Transform _enemySpawnPointContainer;

        [FoldoutGroup("FreezParticle")] [SerializeField]
        private FreezParticleMarker _freezParticlePrefab;

        [FoldoutGroup("FreezParticle")] [SerializeField]
        private Transform _freezParticleContainer;

        [FoldoutGroup("PlayerArrow")] [SerializeField]
        private PlayerArrowDamage _playerArrowDamage;

        [FoldoutGroup("PlayerArrow")] [SerializeField]
        private Transform _playerArrowDamageContainer;

        [FoldoutGroup("MinimapNotifier")] [SerializeField]
        private BarricadeAttackedMinimap _barricadeAttackedMinimap;

        [FoldoutGroup("MinimapNotifier")] [SerializeField]
        private BarricadeDestroyedMinimap _barricadeDestroyedMinimap;

        [FoldoutGroup("MinimapNotifier")] [SerializeField]
        private Transform _minimapContainer;


        public override void InstallBindings()
        {
            BindPlayerArrowPool();

            BindUnitArrowPool();

            BindCatapultBulletPool();

            BindFearBulletPool();

            BindTooltipPool();

            BindFencePool();

            BindCoinLootPool();

            BindBagPool();

            BindEnemyCrystalPool();

            BindFreezParticlePool();

            BindMinimapNotifierPool();
        }

        private void BindMinimapNotifierPool()
        {
            Container
                .BindInterfacesAndSelfTo<PoolObjects<BarricadeAttackedMinimap>>()
                .AsSingle()
                .WithArguments(_barricadeAttackedMinimap, _minimapContainer);

            Container
                .BindInterfacesAndSelfTo<PoolObjects<BarricadeDestroyedMinimap>>()
                .AsSingle()
                .WithArguments(_barricadeDestroyedMinimap, _minimapContainer);
        }

        private void BindPlayerArrowPool()
        {
            Container
                .BindInterfacesAndSelfTo<PoolObjects<PlayerArrowDamage>>()
                .AsSingle()
                .WithArguments(_playerArrowDamage, _playerArrowDamageContainer);
        }

        private void BindFreezParticlePool()
        {
            Container
                .BindInterfacesAndSelfTo<PoolObjects<FreezParticleMarker>>()
                .AsSingle()
                .WithArguments(_freezParticlePrefab, _freezParticleContainer);
        }

        private void BindEnemyCrystalPool()
        {
            Container
                .BindInterfacesAndSelfTo<PoolObjects<EnemySpawnPoint>>()
                .AsSingle()
                .WithArguments(_enemySpawnPointPrefab, _enemySpawnPointContainer);
        }

        private void BindBagPool()
        {
            Container
                .BindInterfacesAndSelfTo<PoolObjects<BagOfMoney>>()
                .AsSingle()
                .WithArguments(_bagPrefab, _bagContainer);
        }

        private void BindCoinLootPool()
        {
            Container
                .BindInterfacesAndSelfTo<PoolObjects<CoinLoot>>()
                .AsSingle()
                .WithArguments(_coinLootCoinPrefab, _lootCoinContainer);
        }

        private void BindFencePool()
        {
            Container
                .BindInterfacesAndSelfTo<PoolObjects<LeftFenceMarker>>()
                .AsSingle()
                .WithArguments(_leftFencePoolPrefab, _fenceContainer);

            Container
                .BindInterfacesAndSelfTo<PoolObjects<RightFenceMarker>>()
                .AsSingle()
                .WithArguments(_rightFencePoolPrefab, _fenceContainer);
        }

        private void BindUnitArrowPool()
        {
            Container
                .BindInterfacesAndSelfTo<PoolObjects<ArrowDamage>>()
                .AsSingle()
                .WithArguments(_arrowPoolPrefab, _arrowContainer);
        }

        private void BindCatapultBulletPool()
        {
            Container
                .BindInterfacesAndSelfTo<PoolObjects<CatapultBullet>>()
                .AsSingle()
                .WithArguments(_catapultBulletPoolPrefab, _catapultBulletContainer);
        }

        private void BindFearBulletPool()
        {
            Container
                .BindInterfacesAndSelfTo<PoolObjects<FearBullet>>()
                .AsSingle()
                .WithArguments(_fearBulletPoolPrefab, _fearBulletContainer);
        }

        private void BindTooltipPool()
        {
            Container
                .BindInterfacesAndSelfTo<PoolObjects<TooltipWorld>>()
                .AsSingle()
                .WithArguments(_tooltipWorldPoolPrefab, _tooltipWorldContainer);
        }
    }
}