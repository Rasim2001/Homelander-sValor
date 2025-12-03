using Infastructure.StaticData.EnemyCristal;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(EnemyCristalMarker))]
    public class EnemyCristalMarkerEditor : UnityEditor.Editor
    {
        [DrawGizmo(GizmoType.Active | GizmoType.Pickable | GizmoType.NonSelected)]
        public static void RenderCustomGizmo(EnemyCristalMarker spawner, GizmoType gizmo)
        {
            /*Gizmos.color = Color.red;
            Gizmos.DrawSphere(spawner.transform.position, 0.5f);*/
        }
    }
}