using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Infastructure.Services.Pool;
using MinimapCore;
using UnityEngine;

namespace Infastructure.Services.MinimapManagement
{
    public class MinimapNotifierService : IMinimapNotifierService
    {
        private const float ATTACKED_HEIGHT_OFFSET = 3f;
        private const float DESTROYED_HEIGHT_OFFSET = 0.3f;
        private const float DESTROYED_SHOW_DURATION = 1f;

        private readonly Dictionary<string, BarricadeAttackedMinimap> _barricadeAttackedDictionary =
            new Dictionary<string, BarricadeAttackedMinimap>();

        private readonly IPoolObjects<BarricadeAttackedMinimap> _attackedPool;
        private readonly IPoolObjects<BarricadeDestroyedMinimap> _destroyedPool;

        public MinimapNotifierService(
            IPoolObjects<BarricadeAttackedMinimap> attackedPool,
            IPoolObjects<BarricadeDestroyedMinimap> destroyedPool)
        {
            _attackedPool = attackedPool;
            _destroyedPool = destroyedPool;
        }

        public void BarricadeAttackedNotify(string uniqueId, Vector3 position)
        {
            if (_barricadeAttackedDictionary.ContainsKey(uniqueId))
                return;

            Vector3 targetPosition = position + new Vector3(0, ATTACKED_HEIGHT_OFFSET);

            BarricadeAttackedMinimap barricadeAttackedMinimap = _attackedPool.GetObjectFromPool();
            barricadeAttackedMinimap.transform.position = targetPosition;
            barricadeAttackedMinimap.EnableObject();

            _barricadeAttackedDictionary.Add(uniqueId, barricadeAttackedMinimap);
        }

        public void BarricadeAttackedFinishedNotify(string uniqueId)
        {
            if (!_barricadeAttackedDictionary.TryGetValue(uniqueId,
                    out BarricadeAttackedMinimap barricadeAttackedMinimap))
                return;

            barricadeAttackedMinimap.DisableObject(
                () => _attackedPool.ReturnObjectToPool(barricadeAttackedMinimap));

            _barricadeAttackedDictionary.Remove(uniqueId);
        }

        public void BarricadeDestroyedNotify(Vector3 position) =>
            ShowDestroyedBarricade(position).Forget();


        private async UniTask ShowDestroyedBarricade(Vector3 position)
        {
            BarricadeDestroyedMinimap barricadeDestroyedMinimap = _destroyedPool.GetObjectFromPool();
            barricadeDestroyedMinimap.transform.position = position - new Vector3(0, DESTROYED_HEIGHT_OFFSET, 0);

            await UniTask.Delay(TimeSpan.FromSeconds(DESTROYED_SHOW_DURATION));
            await barricadeDestroyedMinimap.ShowAsync();

            _destroyedPool.ReturnObjectToPool(barricadeDestroyedMinimap);
        }
    }
}