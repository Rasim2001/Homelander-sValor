using UnityEngine;

namespace Infastructure.Services.AssetProvider
{
    public interface IAssetProviderService
    {
        GameObject Instantiate(string path);
        GameObject Instantiate(string path, Transform parent);
        GameObject Instantiate(GameObject prefab, Transform parent);
    }
}