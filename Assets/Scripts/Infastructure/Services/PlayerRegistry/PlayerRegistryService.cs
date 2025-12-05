using System;
using UnityEngine;

namespace Infastructure.Services.PlayerRegistry
{
    public class PlayerRegistryService : IPlayerRegistryService
    {
        public event Action OnInitialized;

        public GameObject Player
        {
            get => _player;
            set
            {
                _player = value;

                OnInitialized?.Invoke();
            }
        }
        private GameObject _player;
    }
}