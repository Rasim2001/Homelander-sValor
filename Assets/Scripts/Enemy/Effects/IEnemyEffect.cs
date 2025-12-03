using System;
using System.Collections.Generic;

namespace Enemy.Effects
{
    public interface IEnemyEffect
    {
        bool IsWorked { get; }
        void ApplyEffect();
        void RemoveEffect();
        List<Type> GetOverridableEffects();
        List<EnemyTypeId> GetUnaffectedUnits();
    }
}