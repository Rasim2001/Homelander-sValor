using Plugins.Sprite_Shaders_Ultimate.Scripts.Internal;
using UnityEngine;
using UnityEngine.UI;

namespace Plugins.Sprite_Shaders_Ultimate.Scripts
{
    [AddComponentMenu("Sprite Shaders Ultimate/Utility/Material Instancer SSU")]
    public class MaterialInstancerSSU : InstancerSSU
    {
        void Awake()
        {
            Graphic graphic = GetComponent<Graphic>();
            if(graphic != null)
            {
                graphic.material = Instantiate(graphic.material);
                runtimeMaterial = graphic.materialForRendering;
            }

            Renderer renderer = GetComponent<Renderer>();
            if(renderer != null)
            {
                Material[] materials = renderer.sharedMaterials;
                for(int n = 0; n < materials.Length; n++)
                {
                    materials[n] = Instantiate(materials[n]);
                }

                renderer.materials = renderer.sharedMaterials = materials;
                runtimeMaterial = materials[0];
            }
        }
    }
}
