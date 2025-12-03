using System;
using UnityEngine;

namespace Infastructure.StaticData.CardsData
{
    [Serializable]
    public class UpgradeCardData
    {
        public CardId CardId;

        public string Name;
        public string Description;
        public Sprite Icon;
    }
}