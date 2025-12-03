using System;
using Player;

namespace Infastructure.Services.BuildModeServices
{
    public interface IBuildingModifyService
    {
        void Initialize(ShowPriceZone showPriceZone, ObserverTrigger observerTrigger);
        void StartModify();
        void ExitModify();
        bool IsActive { get; }
        event Action OnActiveChanged;
    }
}