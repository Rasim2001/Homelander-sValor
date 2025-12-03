using UnityEngine;

namespace BuildProcessManagement.Towers.CatapultTower
{
    public interface IBowl
    {
        void PlayShootAnimation();
        void SetTarget(Transform target);
        void SetFlipX(bool value);
    }
}