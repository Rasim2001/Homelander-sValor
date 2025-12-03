using UnityEngine;

namespace Infastructure.Services.AssetProvider
{
    public class AssetProviderService : IAssetProviderService
    {
        public GameObject Instantiate(string path) =>
            Object.Instantiate(Resources.Load<GameObject>(path));

        public GameObject Instantiate(string path, Transform parent) =>
            Object.Instantiate(Resources.Load<GameObject>(path), parent);

        public GameObject Instantiate(GameObject prefab, Transform parent) =>
            Object.Instantiate(prefab, parent);
    }
}