using System;
using UnityEngine;

namespace Units.Animators
{
    public class BuilderActionHandler : MonoBehaviour
    {
        public Action OnWorkHappened;
        public Action OnWorkFinishHappened;

        public void WorkAction() =>
            OnWorkHappened?.Invoke();

        public void WorkFinishAction() =>
            OnWorkFinishHappened?.Invoke();
    }
}