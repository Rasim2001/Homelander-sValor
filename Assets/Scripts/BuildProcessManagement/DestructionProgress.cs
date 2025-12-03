using BuildProcessManagement.Towers;

namespace BuildProcessManagement
{
    public class DestructionProgress : BaseDestruction
    {
        public void UpdateDestructionProgress(float healthRatio)
        {
            _destruction.ProgressDestruction = 1 - healthRatio;

            _destruction.ShakeBuilding();
            ModifyDestructionBuilding();
        }
    }
}