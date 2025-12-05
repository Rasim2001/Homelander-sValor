namespace Infastructure.Services.Tutorial.TutorialProgress
{
    public interface ITutorialProgressService
    {
        bool TutorialStarted { get; set; }
        bool ReadyToUseAcceleration { get; set; }
        bool IsCallUnitsReadyToUse { get; set; }
        bool IsCastSkillReadyToUse { get; set; }
        bool IsReleaseUnitsReadyToUse { get; set; }
        bool IsSelectUnitsReadyToUse { get; set; }
        bool IsAttackReadyToUse { get; set; }
        bool IsGiveOrderReadyToUse { get; set; }
        bool IsBuildingStateReadyToUse { get; set; }
        bool IsCallingNightReadyToUse { get; set; }
    }
}