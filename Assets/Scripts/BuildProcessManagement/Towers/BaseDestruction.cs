using UnityEngine;

namespace BuildProcessManagement.Towers
{
    public abstract class BaseDestruction : MonoBehaviour
    {
        [SerializeField] protected DestructionStore _destruction;

        protected void ModifyDestructionBuilding()
        {
            if (_destruction.DestructionInfos.Count == 0 ||
                _destruction.AmountOfDestructionUpdates >= _destruction.DestructionInfos.Count)
                return;


            if (_destruction.ProgressDestruction >=
                _destruction.DestructionInfos[_destruction.AmountOfDestructionUpdates].ProgressPercent)
            {
                _destruction.SpriteRender.sprite =
                    _destruction.DestructionInfos[_destruction.AmountOfDestructionUpdates].Sprite;
                _destruction.AmountOfDestructionUpdates++;
            }
        }
    }
}