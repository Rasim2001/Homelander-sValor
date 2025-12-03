using DG.Tweening;
using UnityEngine;

namespace UI.Windows
{
    public class MenuSettingsWindow : WindowBase
    {
        [SerializeField] private Transform _container;

        protected override void Initialize()
        {
            _container.localScale = Vector3.zero;
            _container.DOScale(Vector3.one, 1);
        }
    }
}