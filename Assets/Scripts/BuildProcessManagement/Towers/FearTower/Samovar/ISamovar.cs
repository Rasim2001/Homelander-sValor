using UnityEngine;

namespace BuildProcessManagement.Towers.FearTower.Samovar
{
    public interface ISamovar
    {
        void SetCharge();
        void PlayShootAnimation();
        Transform SpawnPoint { get; }
    }
}