using UnityEngine;

namespace Infastructure.StaticData.Player
{
    [CreateAssetMenu(fileName = "PlayerStaticData", menuName = "StaticData/Player")]
    public class PlayerStaticData : ScriptableObject
    {
        public float Speed = 5;
        public float AccelerationTime = 10;
        public float BuildModeDelay = 0.5f;

        public int ShootDamage;
        public int Hp;
        public int AmountOfCoins;
    }
}