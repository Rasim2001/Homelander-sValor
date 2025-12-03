using Infastructure.StaticData.Windows;
using UnityEngine;

namespace Infastructure.Services.Window.GameWindowService
{
    public interface IGameWindowService : IWindowService
    {
        GameObject OpenAndGet(WindowId windowId);
    }
}