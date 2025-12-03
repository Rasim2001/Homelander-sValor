using Enemy.Effects;
using Enemy.Effects.Miss;
using Enemy.Effects.Slowdown;

namespace BuildProcessManagement.Towers.BallistaTar
{
    public class TarLiquidSecond : TarLiquidBase
    {
        protected override void AddAllEffects(IEnemyEffectSystem effectSystem)
        {
            effectSystem.AddEffect<SlowdownEffect>();
            effectSystem.AddEffect<MissEffect>();
        }
    }
}