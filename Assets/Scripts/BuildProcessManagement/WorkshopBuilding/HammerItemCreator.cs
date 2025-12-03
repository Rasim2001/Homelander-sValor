using System.Collections.Generic;
using BuildProcessManagement.WorkshopBuilding.Product;
using Infastructure.Services.Pool;
using Infastructure.StaticData.DefaultMaterial;
using UnityEngine;
using Zenject;

namespace BuildProcessManagement.WorkshopBuilding
{
    public class HammerItemCreator : IWorkshopItemCreator
    {
        private readonly List<HammerObject> _hammersList = new List<HammerObject>();
        private readonly IPoolObjects<HammerObject> _hammers;
        private readonly DefaultMaterialStaticData _defaultMaterialStaticData;

        public HammerItemCreator(IPoolObjects<HammerObject> hammers,
            DefaultMaterialStaticData defaultMaterialStaticData)
        {
            _hammers = hammers;
            _defaultMaterialStaticData = defaultMaterialStaticData;
        }


        public void Reduce()
        {
            HammerObject lastObject = _hammersList[^1];
            _hammers.ReturnObjectToPool(lastObject);
            _hammersList.Remove(lastObject);
        }

        public void CreateItem(Vector3 positon)
        {
            HammerObject hammerObject = _hammers.GetObjectFromPool();
            hammerObject.transform.position = positon;
            
            hammerObject.Initialize(_defaultMaterialStaticData.UnlitMaterial, _defaultMaterialStaticData.LitMaterial);
            hammerObject.Illuminate();

            _hammersList.Add(hammerObject);
        }
    }
}