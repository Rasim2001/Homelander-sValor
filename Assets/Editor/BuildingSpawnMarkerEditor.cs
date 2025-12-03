using BuildProcessManagement.SpawnMarker;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(BuildingSpawnerMarker))]
    public class BuildingSpawnMarkerEditor : UnityEditor.Editor
    {
        [DrawGizmo(GizmoType.Active | GizmoType.Pickable | GizmoType.NonSelected)]
        public static void RenderCustomGizmo(BuildingSpawnerMarker spawner, GizmoType gizmo)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(spawner.transform.position, 0.2f);
        }
    }
}