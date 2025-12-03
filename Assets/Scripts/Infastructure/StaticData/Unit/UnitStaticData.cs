using Sirenix.OdinInspector;
using UnityEngine;

namespace Infastructure.StaticData.Unit
{
    [CreateAssetMenu(fileName = "UnitData", menuName = "StaticData/Unit")]
    public class UnitStaticData : ScriptableObject
    {
        public GameObject Prefab;
        public UnitTypeId UnitTypeId;


        [ShowIf("IsWarrior")] public int Hp;

        [ShowIf("IsUnitAttacker")] public int Damage;

        [ShowIf("IsPlayerUnits")] [FoldoutGroup("UnitMoveGroup")]
        public float MinimalDistanceMove = 2;

        [ShowIf("IsPlayerUnits")] [FoldoutGroup("UnitMoveGroup/MoveStates")]
        public float WalkSpeed = 1;
        [ShowIf("IsPlayerUnits")] [FoldoutGroup("UnitMoveGroup/MoveStates")] [LabelText("@GetRunSpeedLabel()")]
        public float RunSpeed = 2;
        [ShowIf("IsPlayerUnitsAndNotVagabond")] [FoldoutGroup("UnitMoveGroup/MoveStates")]
        public float RunTowardPlayerSpeed = 2;
        [ShowIf("IsPlayerUnitsAndNotVagabond")] [FoldoutGroup("UnitMoveGroup/MoveStates")]
        public float FastRunTowardPlayerSpeed = 4;
        [ShowIf("IsPlayerUnitsAndNotVagabond")] [FoldoutGroup("UnitMoveGroup/MoveStates")]
        public float RetreatSpeed = 2;

        [ShowIf("IsPlayerUnits")] [FoldoutGroup("UnitMoveGroup/FollowSpeed")]
        public float MinAdditionalFollowSpeed = -0.5f;
        [ShowIf("IsPlayerUnits")] [FoldoutGroup("UnitMoveGroup/FollowSpeed")]
        public float MaxAdditionalFollowSpeed = 0.25f;

        [ShowIf("IsPlayerUnits")] [FoldoutGroup("UnitMoveGroup/AnimationSpeed")]
        public float MinAnimationSpeed = 0.75f;
        [ShowIf("IsPlayerUnits")] [FoldoutGroup("UnitMoveGroup/AnimationSpeed")]
        public float MaxAnimationSpeed = 1.5f;


        private bool IsWarrior() =>
            UnitTypeId == UnitTypeId.Archer || UnitTypeId == UnitTypeId.Shielder || UnitTypeId == UnitTypeId.Builder;

        private bool IsPlayerUnits() =>
            UnitTypeId == UnitTypeId.Archer ||
            UnitTypeId == UnitTypeId.Shielder ||
            UnitTypeId == UnitTypeId.Homeless ||
            UnitTypeId == UnitTypeId.Builder ||
            UnitTypeId == UnitTypeId.Vagabond;

        private bool IsPlayerUnitsAndNotVagabond() =>
            UnitTypeId == UnitTypeId.Archer ||
            UnitTypeId == UnitTypeId.Shielder ||
            UnitTypeId == UnitTypeId.Homeless ||
            UnitTypeId == UnitTypeId.Builder;

        private string GetRunSpeedLabel() =>
            IsVagabond() ? "ScaryRunSpeed" : "RunSpeed";

        private bool IsUnitAttacker() =>
            UnitTypeId == UnitTypeId.Archer ||
            UnitTypeId == UnitTypeId.Shielder ||
            UnitTypeId == UnitTypeId.Marksman ||
            UnitTypeId == UnitTypeId.Catapultman ||
            UnitTypeId == UnitTypeId.Ballistaman;

        private bool IsVagabond() =>
            UnitTypeId == UnitTypeId.Vagabond;
    }
}