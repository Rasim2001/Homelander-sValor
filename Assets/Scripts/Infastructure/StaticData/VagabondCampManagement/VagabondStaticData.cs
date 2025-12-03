using UnityEngine;

namespace Infastructure.StaticData.VagabondCampManagement
{
    [CreateAssetMenu(fileName = "VagabondCamp", menuName = "StaticData/VagabondCamp")]
    public class VagabondStaticData : ScriptableObject
    {
        public int RequiredVagabonds;
        public float TimeSpawn;
    }
}