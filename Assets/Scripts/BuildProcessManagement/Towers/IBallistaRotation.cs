using System;
using UnityEngine;

namespace BuildProcessManagement.Towers
{
    public interface IBallistaRotation
    {
        void SetTarget(Collider2D target);
        bool HasReachedTargetRotation();
        void SetTargetLock();
        void UnlockTarget();
        void Initialize(int deltaAngle);
        event Action OnTargetDeath;
    }
}