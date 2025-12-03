using UnityEngine;

namespace Infastructure.StaticData.DefaultMaterial
{
    [CreateAssetMenu(fileName = "DefaultMaterialData", menuName = "StaticData/DefaultMaterialData")]
    public class DefaultMaterialStaticData : ScriptableObject
    {
        public Material UnlitMaterial;
        public Material LitMaterial;

        public Material EnemyDeathMaterial;
    }
}