using System;
using UnityEngine;

namespace Units.Vagabond
{
    public class VagabondDeath : MonoBehaviour
    {
        public Action<VagabondDeath> OnDeathHappened;

        public void Die()
        {
            OnDeathHappened?.Invoke(this);

            Destroy(gameObject);
        }
    }
}