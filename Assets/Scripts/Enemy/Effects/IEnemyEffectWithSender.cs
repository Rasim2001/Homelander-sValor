using UnityEngine;

namespace Enemy.Effects
{
    public interface IEnemyEffectWithSender : IEnemyEffect
    {
        public Transform SenderTransform { get; set; }
    }
}