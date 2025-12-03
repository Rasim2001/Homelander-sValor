using Infastructure.StaticData.EnemyCristal;
using UnityEditor;
using UnityEngine;

namespace Editor.EnemyCristal
{
    [CustomEditor(typeof(EnemyCristalMarker))]
    public class EnemyCrystalEditor : UnityEditor.Editor
    {
        [DrawGizmo(GizmoType.Active | GizmoType.Pickable | GizmoType.NonSelected)]
        public static void RenderCustomGizmo(EnemyCristalMarker marker, GizmoType gizmo)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(marker.transform.position, 0.2f);
        }
    }
}