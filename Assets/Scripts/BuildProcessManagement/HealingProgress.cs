using UnityEngine;

namespace BuildProcessManagement
{
    public class HealingProgress : MonoBehaviour
    {
        [SerializeField] private DestructionStore _destructionStore;
        [SerializeField] private ParticleSystem _healFx;

        public void UpdateHealProgress(float healthRatio)
        {
            _destructionStore.ProgressDestruction = 1 - healthRatio;

            _destructionStore.ShakeBuilding();
            ModifyHealingProgress();
        }

        public void PlayHealFx() =>
            _healFx.Play();

        public void StopHealFx() =>
            _healFx.Stop();

        public void RefreshHeal() =>
            _destructionStore.ProgressDestruction = 0;

        public bool HealIsFinished() =>
            _destructionStore.ProgressDestruction <= 0;

        private void ModifyHealingProgress()
        {
            for (int i = 0; i < _destructionStore.DestructionInfos.Count; i++)
            {
                if (_destructionStore.ProgressDestruction < _destructionStore.DestructionInfos[i].ProgressPercent)
                {
                    int index = i - 1 >= 0 ? i - 1 : 0;
                    _destructionStore.SpriteRender.sprite = _destructionStore.DestructionInfos[index].Sprite;
                    _destructionStore.AmountOfDestructionUpdates = i;
                    break;
                }
            }
        }
    }
}