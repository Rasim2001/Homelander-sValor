using UnityEngine;

namespace Plugins.Sprite_Shaders_Ultimate.Scripts.Internal
{
    [DisallowMultipleComponent()]
    public class InstancerSSU : MonoBehaviour
    {
        //To prevent multiple components of subtypes.
        [HideInInspector]
        public Material runtimeMaterial;
    }
}
