using System;
using System.Collections.Generic;
using System.Linq;
using BuildProcessManagement.SpawnMarker;
using Enemy.SpawnMarker;
using Infastructure.StaticData;
using Infastructure.StaticData.Building;
using Infastructure.StaticData.Enemy;
using Infastructure.StaticData.EnemyCristal;
using Infastructure.StaticData.Forest;
using Infastructure.StaticData.RecourceElements;
using Infastructure.StaticData.VagabondCampManagement;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(GameStaticData))]
    public class GameDataEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GameStaticData gameData = (GameStaticData)target;

            if (GUILayout.Button("Collect"))
            {
                gameData.EnemyCamps =
                    FindObjectsOfType<EnemyCampMarker>()
                        .Select(x => new EnemyCampData(x.transform.position, x.MicroWaveCamp, x.Hp, GetUniqueId(x)))
                        .ToList();

                gameData.VagabondCampDatas =
                    FindObjectsOfType<VagabondCampMarker>()
                        .Select(x => new VagabondCampData(x.transform.position, GetUniqueId(x)))
                        .ToList();


                gameData.BuildingSpawners =
                    FindObjectsOfType<BuildingSpawnerMarker>()
                        .Select(x => new BuildingSpawnerData(x.transform.position, x._buildingTypeId, GetUniqueId(x)))
                        .ToList();

                gameData.ResourceSpawners =
                    FindObjectsOfType<ResourceSpawnerMarker>()
                        .Select(x => new ResourceSpawnerData(GetUniqueId(x), x.transform.position, x.ResourceId))
                        .ToList();

                gameData.ForestSides =
                    FindObjectsOfType<ForestMarker>()
                        .Select(x => new ForestData(
                            x.transform.position,
                            x.GetComponent<BoxCollider2D>().offset,
                            x.GetComponent<BoxCollider2D>().size))
                        .ToList();

                gameData.EnemyCristalConfigs =
                    FindObjectsOfType<EnemyCristalMarker>()
                        .Select(x =>
                            new EnemyCristalConfig(GetUniqueId(x), x.transform.position,
                                new List<EnemyCristalData>(x.Configs)))
                        .ToList();
            }

            EditorUtility.SetDirty(target);
        }

        private string GetUniqueId(Marker marker)
        {
            marker.UniqueId = string.IsNullOrEmpty(marker.UniqueId)
                ? Guid.NewGuid().ToString()
                : marker.UniqueId;

            return marker.UniqueId;
        }
    }
}