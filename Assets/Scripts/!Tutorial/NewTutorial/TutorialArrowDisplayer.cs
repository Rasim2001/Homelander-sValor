using System;
using System.Collections.Generic;
using Infastructure.Services.Pool;
using UnityEngine;
using Zenject;

namespace _Tutorial.NewTutorial
{
    public class TutorialArrowDisplayer : MonoBehaviour, ITutorialArrowDisplayer
    {
        private readonly Dictionary<int, TutorialArrowView> _arrowDictionary =
            new Dictionary<int, TutorialArrowView>();

        private IPoolObjects<TutorialArrowView> _poolObjects;

        [Inject]
        public void Construct(IPoolObjects<TutorialArrowView> poolObjects) =>
            _poolObjects = poolObjects;

        public void Show(Transform target)
        {
            int id = target.GetInstanceID();

            if (_arrowDictionary.ContainsKey(id))
                return;

            TutorialArrowView tutorialArrowView = _poolObjects.GetObjectFromPool();
            tutorialArrowView.transform.SetParent(transform);
            tutorialArrowView.Initialize(target);

            _arrowDictionary.Add(id, tutorialArrowView);
        }

        public void Hide(Transform target)
        {
            int id = target.GetInstanceID();

            if (!_arrowDictionary.TryGetValue(id, out TutorialArrowView tutorialArrowView))
                return;

            _poolObjects.ReturnObjectToPool(tutorialArrowView);
            _arrowDictionary.Remove(id);
        }
    }
}