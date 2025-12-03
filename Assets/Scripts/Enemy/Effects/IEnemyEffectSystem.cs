using UnityEngine;

namespace Enemy.Effects
{
    public interface IEnemyEffectSystem
    {
        void AddEffect<T>(float duration = 5) where T : IEnemyEffect;
        void RemoveEffect<T>() where T : IEnemyEffect;
        void AddEffect<T>(Transform sender, float duration = 5) where T : IEnemyEffectWithSender;
    }
}