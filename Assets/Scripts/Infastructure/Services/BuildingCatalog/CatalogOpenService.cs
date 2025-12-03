using System.Collections.Generic;
using Infastructure.Services.InputPlayerService;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Infastructure.Services.BuildingCatalog
{
    public class CatalogOpenService : ICatalogOpenService, ITickable
    {
        private readonly IInputService _inputService;

        private ICatalog _currentCatalog;

        public CatalogOpenService(IInputService inputService) =>
            _inputService = inputService;

        public void Tick()
        {
            if (_inputService.MouseClicked)
            {
                GameObject uiObject = GetClickedUIObject();
                if (uiObject == null)
                    return;

                ICatalog catalog = uiObject.GetComponentInParent<ICatalog>();

                if (catalog == null && _currentCatalog != null)
                {
                    _currentCatalog.CloseCatalog();
                    _currentCatalog = null;
                }
            }
        }

        public void ToggleCatalog(ICatalog catalog)
        {
            if (_currentCatalog == catalog)
            {
                _currentCatalog.CloseCatalog();
                _currentCatalog = null;
            }
            else
            {
                if (_currentCatalog != null)
                    _currentCatalog.CloseCatalog();

                _currentCatalog = catalog;
                _currentCatalog.OpenCatalog();
            }
        }

        private GameObject GetClickedUIObject()
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            if (results.Count > 0)
                return results[0].gameObject;

            return null;
        }
    }
}