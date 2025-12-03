using UnityEngine;

namespace Units
{
    public class RangeAttackOption : AttackOptionBase
    {
        [SerializeField] private GameObject _rangeKeeper;

        public override void SetAttackZone(bool value)
        {
            base.SetAttackZone(value);

            _rangeKeeper.SetActive(value);
        }


        public override void PrepareDefense()
        {
            base.PrepareDefense();

            _rangeKeeper.SetActive(false);
        }
    }
}