using System.Collections.Generic;
using BuildProcessManagement.WorkshopBuilding.Product;
using Infastructure.Services.Pool;
using Infastructure.StaticData.DefaultMaterial;
using UnityEngine;

namespace BuildProcessManagement.WorkshopBuilding
{
    public class ArrowItemCreator : IWorkshopItemCreator
    {
        private readonly List<ArrowObject> _arrowsList = new List<ArrowObject>();
        private readonly IPoolObjects<ArrowObject> _arrows;
        private readonly DefaultMaterialStaticData _defaultMaterialStaticData;

        public ArrowItemCreator(IPoolObjects<ArrowObject> arrows, DefaultMaterialStaticData defaultMaterialStaticData)
        {
            _arrows = arrows;
            _defaultMaterialStaticData = defaultMaterialStaticData;
        }

        public void Reduce()
        {
            ArrowObject lastObject = _arrowsList[^1];
            _arrows.ReturnObjectToPool(lastObject);
            _arrowsList.Remove(lastObject);
        }

        public void CreateItem(Vector3 position)
        {
            ArrowObject arrow = _arrows.GetObjectFromPool();
            arrow.transform.position = position;

            arrow.Initialize(_defaultMaterialStaticData.UnlitMaterial, _defaultMaterialStaticData.LitMaterial);
            arrow.Illuminate();

            _arrowsList.Add(arrow);
        }
    }
}