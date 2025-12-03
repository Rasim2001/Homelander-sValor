using BuildProcessManagement.SpawnMarker;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(ResourceSpawnerMarker))]
    public class ResourceSpawnerMarkerEditor : UnityEditor.Editor
    {
        [DrawGizmo(GizmoType.Active | GizmoType.Pickable | GizmoType.NonSelected)]
        public static void RenderCustomGizmo(ResourceSpawnerMarker spawner, GizmoType gizmo)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(spawner.transform.position, 0.2f);
        }
    }
}