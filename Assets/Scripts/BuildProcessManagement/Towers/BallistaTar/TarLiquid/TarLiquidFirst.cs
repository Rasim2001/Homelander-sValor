using Enemy.Effects;
using Enemy.Effects.Slowdown;

namespace BuildProcessManagement.Towers.BallistaTar
{
    public class TarLiquidFirst : TarLiquidBase
    {
        protected override void AddAllEffects(IEnemyEffectSystem effectSystem) =>
            effectSystem.AddEffect<SlowdownEffect>();
    }
}