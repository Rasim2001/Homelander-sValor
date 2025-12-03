using UnityEngine;

namespace Units.UnitStates
{
    public static class UnitStatesPath
    {
        public static readonly int IdleHash = Animator.StringToHash("Idle");
        public static readonly int WalkHash = Animator.StringToHash("Walk");
        public static readonly int FastRunHash = Animator.StringToHash("FastRun");
        public static readonly int RunHash = Animator.StringToHash("Run");

        public static readonly int WorkHash = Animator.StringToHash("Work");
        public static readonly int WorkFinishedHash = Animator.StringToHash("WorkFinished");
        public static readonly int AttackHash = Animator.StringToHash("Attack");
        public static readonly int VendorHappyHash = Animator.StringToHash("Happy");
        public static readonly int ShielderRetreatHash = Animator.StringToHash("Retreat");

        public static readonly int ShootHash = Animator.StringToHash("Shoot");
        public static readonly int ReloadHash = Animator.StringToHash("Reload");
    }
}