using System;
using System.Collections.Generic;
using System.Linq;
using Enemy.Effects.ArmorBreak;
using Enemy.Effects.Bleed;
using Enemy.Effects.Convert;
using Enemy.Effects.Damage;
using Enemy.Effects.Fear;
using Enemy.Effects.Miss;
using Enemy.Effects.Slowdown;
using Enemy.Effects.Stun;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Infastructure.StaticData.EffectsUI
{
    [Serializable]
    public class EffectEntry
    {
        [SerializeField] [ValueDropdown("GetEffectTypeNames")]
        private string _effectTypeName;

        [SerializeField] private Sprite _effectSprite;

        private readonly Type[] _availableEffectTypes = new Type[]
        {
            typeof(FearEffect),
            typeof(SlowdownEffect),
            typeof(MissEffect),
            typeof(ConvertEffect),
            typeof(StunEffect),
            typeof(FireEffect),
            typeof(ArmorBreakEffect),
            typeof(BleedEffect)
        };

        public Sprite EffectSprite => _effectSprite;

        public Type EffectType
        {
            get
            {
                if (string.IsNullOrEmpty(_effectTypeName))
                    return null;

                return _availableEffectTypes.FirstOrDefault(t => t.Name == _effectTypeName);
            }
        }

        private IEnumerable<string> GetEffectTypeNames() =>
            _availableEffectTypes.Select(t => t.Name);
    }
}