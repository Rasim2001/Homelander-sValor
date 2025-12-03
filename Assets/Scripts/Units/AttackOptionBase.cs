using UnityEngine;

namespace Units
{
    public abstract class AttackOptionBase : MonoBehaviour
    {
        [SerializeField] private GameObject _aggressionZone;

        public virtual void SetAttackZone(bool value) =>
            _aggressionZone.SetActive(value);

        public virtual void PrepareDefense() =>
            _aggressionZone.SetActive(true);
    }
}