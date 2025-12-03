using Infastructure.StaticData.VagabondCampManagement;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(VagabondCampMarker))]
    public class VagabondCampMarkerEditor : UnityEditor.Editor
    {
        [DrawGizmo(GizmoType.Active | GizmoType.Pickable | GizmoType.NonSelected)]
        public static void RenderCustomGizmo(VagabondCampMarker camp, GizmoType gizmo)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(camp.transform.position, 0.2f);
        }
    }
}