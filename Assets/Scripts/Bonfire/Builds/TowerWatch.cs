using FogOfWar;
using UnityEngine;
using Zenject;

namespace Bonfire.Builds
{
    public class TowerWatch : MonoBehaviour, ISetParent
    {
        private IFogOfWarMinimap _fogOfWarMinimap;

        [Inject]
        public void Construct(IFogOfWarMinimap fogOfWarMinimap) =>
            _fogOfWarMinimap = fogOfWarMinimap;

        public void SetParent(Transform parent)
        {
            transform.SetParent(parent);

            _fogOfWarMinimap.StartUpdatePosition();
        }
    }
}