using DG.Tweening;
using Infastructure.StaticData.SpeachBuble;
using Infastructure.StaticData.SpeachBuble.Player;
using Infastructure.StaticData.StaticDataService;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.GameplayUI.SpeachBubleUI
{
    public class SpeachBuble : MonoBehaviour
    {
        [SerializeField] private Image _speachImage;

        private IStaticDataService _staticDataService;

        [Inject]
        public void Construct(IStaticDataService staticDataService) =>
            _staticDataService = staticDataService;

        public void UpdateSpeach(SpeachBubleId speachId)
        {
            SpeachBubleConfig speachBubleConfig = _staticDataService.ForSpeachBuble(speachId);
            _speachImage.sprite = speachBubleConfig.SpeachBubleSprite;

            DOTween.Kill(_speachImage);

            _speachImage.color = new Color(1, 1, 1, 0);

            _speachImage.DOFade(1, 0.5f);
            _speachImage.transform.DOScale(1f, 0.5f).From(0.5f);
            _speachImage.DOFade(0, 0.5f).SetDelay(2);
        }
    }
}