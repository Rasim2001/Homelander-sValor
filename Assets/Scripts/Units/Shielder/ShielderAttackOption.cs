using Units.RangeUnits;
using UnityEngine;

namespace Units.Shielder
{
    public class ShielderAttackOption : AttackOptionBase
    {
        [SerializeField] private CheckAttackRange _checkAttackRange;
        [SerializeField] private UnitAggressionMove _unitAggressionMove;

        public override void PrepareDefense()
        {
            base.PrepareDefense();

            _unitAggressionMove.SetDefenseDistance();
            _checkAttackRange.SetDefenseDistance();
        }
    }
}