using Enemy;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Infastructure.StaticData.Enemy
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "StaticData/Enemy")]
    public class EnemyStaticData : SerializedScriptableObject
    {
        public EnemyTypeId EnemyTypeId;
        public GameObject Prefab;

        public int Hp;
        public int Damage;
        public float Speed;
    }
}