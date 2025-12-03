using System.Collections.Generic;
using UnityEngine;

namespace Infastructure.Services.Tutorial
{
    public interface ITutorialSpawnService
    {
        void StartSpawn();
        GameObject ChestObject { get; }
        List<GameObject> HomelessList { get; }
        List<GameObject> WarriorsList { get; }
        GameObject FightPointObject { get; }
        GameObject EnemyObject { get; }
    }
}