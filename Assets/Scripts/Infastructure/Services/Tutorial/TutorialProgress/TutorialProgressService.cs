namespace Infastructure.Services.Tutorial.TutorialProgress
{
    public class TutorialProgressService : ITutorialProgressService
    {
        public bool TutorialStarted { get; set; }
        public bool ReadyToUseAcceleration { get; set; }
        public bool IsCallUnitsReadyToUse { get; set; }
        public bool IsCastSkillReadyToUse { get; set; }
        public bool IsReleaseUnitsReadyToUse { get; set; }
        public bool IsSelectUnitsReadyToUse { get; set; }
        public bool IsAttackReadyToUse { get; set; }
        public bool IsGiveOrderReadyToUse { get; set; }
        public bool IsBuildingStateReadyToUse { get; set; }
        public bool IsCallingNightReadyToUse { get; set; }
    }
}