using System;
using DayCycle;
using UnityEngine;

namespace CutScenes
{
    public interface ICristalTimeline
    {
        void Initialize(Transform playerTransform);
        bool IsPlaying { get; }
        Transform CristalTransform { get; }
        Action OnPlayFinishHappened { get; set; }
        Action OnPlayStartHappened { get; set; }
    }
}