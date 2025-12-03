using System;

namespace Infastructure.Data
{
    [Serializable]
    public class PlayerProgress
    {
        public CoinData CoinData;
        public WorldData WorldData;
        public KillData KillData;
        public DayCycleData DayCycleData;
        public UnitDataListWrapper UnitDataListWrapper;
        public FutureOrdersData FutureOrdersData;
        public MainFlagData MainFlagData;
        public CutSceneData CutSceneData;

        public PlayerProgress(int coins)
        {
            CoinData = new CoinData(coins);
            WorldData = new WorldData();
            KillData = new KillData();
            DayCycleData = new DayCycleData();
            UnitDataListWrapper = new UnitDataListWrapper();
            FutureOrdersData = new FutureOrdersData();
            MainFlagData = new MainFlagData();
            CutSceneData = new CutSceneData(true);
        }
    }
}