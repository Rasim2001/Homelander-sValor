using System;
using UnityEngine;

namespace Infastructure.Services.PlayerRegistry
{
    public interface IPlayerRegistryService
    {
        GameObject Player { get; set; }
        event Action OnInitialized;
    }
}