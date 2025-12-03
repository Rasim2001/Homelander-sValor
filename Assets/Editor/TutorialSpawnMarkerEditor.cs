using _Tutorial;
using BuildProcessManagement.SpawnMarker;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(BuildingSpawnerMarker))]
    public class TutorialSpawnMarkerEditor : UnityEditor.Editor
    {
        [DrawGizmo(GizmoType.Active | GizmoType.Pickable | GizmoType.NonSelected)]
        public static void RenderCustomGizmo(TutorialMarker spawner, GizmoType gizmo)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(spawner.transform.position, 0.2f);
        }
    }
}