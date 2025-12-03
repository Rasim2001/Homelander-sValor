using Units.Shielder;
using UnityEngine;

namespace Units.RangeUnits
{
    public class RangeAggressionZone : UnitAggressionZoneBase
    {
        [SerializeField] private UnitAttack _unitAttack;
        [SerializeField] private CheckAttackRange _checkAttackRange;
        [SerializeField] private RangeKeeper _rangeKeeper;

        protected override void Start()
        {
            base.Start();

            _rangeKeeper.OnRangeBreakHappened += TriggerEnter;
        }


        protected override void OnDestroy()
        {
            base.OnDestroy();

            _rangeKeeper.OnRangeBreakHappened -= TriggerEnter;
        }

        protected override void TriggerEnter()
        {
            if (!_rangeKeeper.IsActiveHit || IsAroundOfMainFlag())
            {
                base.TriggerEnter();

                if (_checkAttackRange.HasAnyHits())
                    _unitAttack.EnableAttack();
            }
        }
    }
}