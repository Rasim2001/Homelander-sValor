using DayCycle;
using UnityEngine;

namespace CutScenes
{
    public interface ICristalTimeline
    {
        void Initialize(Transform playerTransform);
        bool IsPlaying { get; }
    }
}