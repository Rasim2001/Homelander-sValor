using System;
using System.Collections.Generic;
using System.Linq;
using Infastructure.StaticData.CardsData;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

namespace Infastructure.StaticData.Building
{
    [Serializable]
    public class BuildingUpgradeData
    {
        [HorizontalGroup("LevelInfo")] public BuildingLevelId LevelId;
        [HorizontalGroup("LevelInfo")] public CardId CardKey;

        public List<CardData> Cards;

        public int HP;
        public int GridSizeX;
        public int CoinsValue;
        public int BuildingTime;

        public GameObject GetPrefabFrom(CardId cardId)
        {
            CardData card = Cards.FirstOrDefault(x => x.CardId == cardId);
            return card?.Prefab;
        }
    }

    [Serializable]
    public class CardData
    {
        [HorizontalGroup("CardInfo")] public CardId CardId;
        [HorizontalGroup("CardInfo")] public GameObject Prefab;
        [ShowIf("IsNotDefaultCardId")] public GameObject CardPrefabUI;

        private bool IsNotDefaultCardId() =>
            CardId != CardId.Default;
    }
}