using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Infastructure.Services.CoinsCreator
{
    public interface ICoinsCreatorService
    {
        UniTask CreateCoinsAsync(Vector3 position, int amount);
    }
}