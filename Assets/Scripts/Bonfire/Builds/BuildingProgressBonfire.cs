using System;
using DG.Tweening;
using UnityEngine;

namespace Bonfire.Builds
{
    public class BuildingProgressBonfire : MonoBehaviour
    {
        [SerializeField] private BonfireInfo _bonfireInfo;

        public int ProgressValue;

        [SerializeField] private float _progress;
        [SerializeField] private int _amountOfUpdates;


        public void Clear()
        {
            _progress = 0;
            _amountOfUpdates = 0;
        }

        public void BuildWoods()
        {
            _progress += (float)_bonfireInfo.WoodsList.Count / ProgressValue;

            ShakeAllWoods(1);

            if (HasUpdate())
            {
                CreateWood();

                _amountOfUpdates++;
                _bonfireInfo.CurrentWoodsCount--;
            }
        }

        public void ShakeAllWoods(int loop, Action onComplete = null) //TODO:
        {
            Sequence sequence = DOTween.Sequence();

            for (int i = 0; i < _bonfireInfo.WoodsList.Count; i++)
            {
                GameObject wood = _bonfireInfo.WoodsList[i];
                Vector3 defaultWoodPosition = wood.transform.position;

                Tweener shakeTween = wood.transform.DOShakePosition(
                        0.1f,
                        new Vector3(0.1f, 0.05f),
                        100,
                        90,
                        false,
                        true,
                        ShakeRandomnessMode.Harmonic)
                    .OnComplete(() => wood.transform.position = defaultWoodPosition);

                sequence.Join(shakeTween);
            }

            sequence.Play()
                .SetLoops(loop)
                .OnComplete(() => onComplete?.Invoke());
        }


        private void CreateWood()
        {
            int index = _bonfireInfo.CurrentWoodsCount - 1;
            if (index >= 0)
            {
                GameObject wood = _bonfireInfo.WoodsList[index];
                wood.SetActive(true);
            }
        }

        private bool HasUpdate() =>
            _progress >= _amountOfUpdates;
    }
}