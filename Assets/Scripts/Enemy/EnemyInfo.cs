using UnityEngine;

namespace Enemy
{
    public class EnemyInfo : MonoBehaviour
    {
        public EnemyTypeId EnemyTypeId { get; set; }
        public MatryoshkaId MatryoshkaId { get; set; } = MatryoshkaId.Unknow;
    }
}