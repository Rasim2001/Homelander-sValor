using Infastructure.StaticData.CardsData;
using UnityEngine;

namespace UI.GameplayUI.Card
{
    public class CardItemUI : MonoBehaviour
    {
        [SerializeField] private GameObject _selectableObejct;

        public CardId CardId;

        public void Select() =>
            _selectableObejct.SetActive(true);

        public void Deselect() =>
            _selectableObejct.SetActive(false);
    }
}