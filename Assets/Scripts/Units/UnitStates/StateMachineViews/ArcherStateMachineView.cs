using Units.Animators;
using Units.UnitStates.DefaultStates;

namespace Units.UnitStates.StateMachineViews
{
    public class ArcherStateMachineView : UnitStateMachineView
    {
        public override void Initialize()
        {
            base.Initialize();

            ArcherAnimator archerAnimator = UnitAnimator as ArcherAnimator;

            AddToCurrentStates(new AttackDefaultState(archerAnimator));
        }
    }
}