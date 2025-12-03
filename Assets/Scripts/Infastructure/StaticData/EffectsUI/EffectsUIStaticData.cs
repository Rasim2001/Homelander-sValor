using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Infastructure.StaticData.EffectsUI
{
    [CreateAssetMenu(fileName = "EffectStaticData", menuName = "StaticData/EffectData")]
    public class EffectsUIStaticData : SerializedScriptableObject
    {
        [SerializeField] private List<EffectEntry> _effects = new List<EffectEntry>();
        public IReadOnlyList<EffectEntry> Effects => _effects.AsReadOnly();
    }
}