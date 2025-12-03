using UnityEngine;

namespace Infastructure.Services.Effects
{
    public interface ISlowEffectService
    {
        void CastEffect(Vector2 position);
    }
}