using UI.GameplayUI.HudUI;
using UnityEngine;

namespace Infastructure.Services.HudFader
{
    public interface IHudFaderService
    {
        void Register(HudId hudId, CanvasGroup canvasGroup);
        void DoFade(HudId hudId);
        void Show(HudId hudId);
        void ShowAll();
        void HideAll();
        void CleanUp();
    }
}