using System.Collections.Generic;
using BuildProcessManagement.WorkshopBuilding.Product;
using Infastructure.Services.Pool;
using Infastructure.StaticData.DefaultMaterial;
using UnityEngine;
using Zenject;

namespace BuildProcessManagement.WorkshopBuilding
{
    public class ShieldItemCreator : IWorkshopItemCreator
    {
        private readonly List<ShieldObject> _shieldList = new List<ShieldObject>();
        private readonly IPoolObjects<ShieldObject> _shields;
        private readonly DefaultMaterialStaticData _defaultMaterialStaticData;

        public ShieldItemCreator(IPoolObjects<ShieldObject> shields,
            DefaultMaterialStaticData defaultMaterialStaticData)
        {
            _shields = shields;
            _defaultMaterialStaticData = defaultMaterialStaticData;
        }

        public void Reduce()
        {
            ShieldObject lastObject = _shieldList[^1];
            _shields.ReturnObjectToPool(lastObject);
            _shieldList.Remove(lastObject);
        }

        public void CreateItem(Vector3 position)
        {
            ShieldObject shield = _shields.GetObjectFromPool();
            shield.transform.position = position;

            shield.Initialize(_defaultMaterialStaticData.UnlitMaterial, _defaultMaterialStaticData.LitMaterial);
            shield.Illuminate();

            _shieldList.Add(shield);
        }
    }
}