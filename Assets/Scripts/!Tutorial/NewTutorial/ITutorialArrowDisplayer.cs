using UnityEngine;

namespace _Tutorial.NewTutorial
{
    public interface ITutorialArrowDisplayer
    {
        void Show(Transform target);
        void Hide(Transform target);
    }
}