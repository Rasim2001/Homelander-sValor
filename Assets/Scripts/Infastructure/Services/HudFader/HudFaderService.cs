using System.Collections.Generic;
using DG.Tweening;
using Infastructure.Services.InputPlayerService;
using UI.GameplayUI.HudUI;
using UnityEngine;
using Zenject;

namespace Infastructure.Services.HudFader
{
    public class HudFaderService : IHudFaderService, ITickable
    {
        private readonly Dictionary<HudId, CanvasGroup> _canvasGroups = new Dictionary<HudId, CanvasGroup>();

        private readonly IInputService _inputService;

        public HudFaderService(IInputService inputService) =>
            _inputService = inputService;

        public void Register(HudId hudId, CanvasGroup canvasGroup) =>
            _canvasGroups.Add(hudId, canvasGroup);


        public void Tick()
        {
            if (_inputService.TabPressed)
            {
                ShowAll();
                HideAll();
            }
        }


        public void CleanUp()
        {
            foreach (CanvasGroup canvasGroup in _canvasGroups.Values)
                canvasGroup?.DOKill();

            _canvasGroups.Clear();
        }


        public void Show(HudId hudId)
        {
            if (!_canvasGroups.TryGetValue(hudId, out var canvasGroup))
                return;

            canvasGroup.blocksRaycasts = true;
            canvasGroup.alpha = 1;
            canvasGroup.DOKill();
        }

        public void DoFade(HudId hudId)
        {
            if (!_canvasGroups.TryGetValue(hudId, out CanvasGroup canvasGroup))
                return;

            canvasGroup?.DOFade(0, 3)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    if (canvasGroup != null)
                        canvasGroup.blocksRaycasts = false;
                });
        }

        public void ShowAll()
        {
            if (_canvasGroups.Count == 0)
                return;

            foreach (HudId hudId in _canvasGroups.Keys)
                Show(hudId);
        }

        public void HideAll()
        {
            if (_canvasGroups.Count == 0)
                return;

            foreach (HudId hudId in _canvasGroups.Keys)
                DoFade(hudId);
        }
    }
}