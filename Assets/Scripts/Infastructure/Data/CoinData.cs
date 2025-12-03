using System;
using System.Collections.Generic;
using System.Linq;

namespace Infastructure.Data
{
    [Serializable]
    public class CoinData
    {
        public List<LootData> LootDatas = new List<LootData>();

        public event Action Changed;
        public int NumberOfCoins;

        public CoinData(int numberOfCoins) =>
            NumberOfCoins = numberOfCoins;

        public void Spend(int value)
        {
            NumberOfCoins -= value;
            Changed?.Invoke();
        }

        public void Collect(int value)
        {
            NumberOfCoins += value;
            Changed?.Invoke();
        }

        public void Collect(int value, string lootUniqueId)
        {
            NumberOfCoins += value;
            Changed?.Invoke();

            LootData lootData = LootDatas.FirstOrDefault(x => x.UniqueId.Contains(lootUniqueId));
            if (lootData != null)
                LootDatas.Remove(lootData);
        }

        public bool IsEnoughCoins(int value) =>
            NumberOfCoins >= value;
    }
}