using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace BuildProcessManagement
{
    public class DestructionStore : MonoBehaviour
    {
        public SpriteRenderer SpriteRender;

        public List<DestructionInfo> DestructionInfos;

        private Tweener _tweenerShakePosition;

        public float ProgressDestruction;
        public int AmountOfDestructionUpdates;

        private Vector3 _startPosition;

        private void Start() =>
            _startPosition = transform.position;

        private void OnDestroy() =>
            _tweenerShakePosition.Kill();

        public void ShakeBuilding()
        {
            if (_tweenerShakePosition != null && _tweenerShakePosition.IsActive())
                _tweenerShakePosition.Kill();


            _tweenerShakePosition = transform.DOShakePosition(
                    0.1f,
                    new Vector3(0.1f, 0.05f),
                    100,
                    90,
                    false,
                    true,
                    ShakeRandomnessMode.Harmonic)
                .OnComplete(() => transform.position = _startPosition);
        }
    }
}