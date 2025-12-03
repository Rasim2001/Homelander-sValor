using System.Globalization;
using UnityEngine;

public class PixelGridDrawer : MonoBehaviour
{
    [Range(1, 25)] [SerializeField] private float _lineHeight;
    [Range(1, 100)] [SerializeField] private int _fontSize;

    [SerializeField] private Color _colorNumbers;
    [SerializeField] private Color _colorGrid;


    private void OnDrawGizmos()
    {
        Gizmos.color = _colorGrid;

        Gizmos.DrawLine(new Vector3(-100, -2.75f, 0), new Vector3(100, -2.75f, 0));
        Gizmos.DrawLine(new Vector3(0, -100, 0), new Vector3(0, 100, 0));

#if UNITY_EDITOR
        Camera mainCamera = UnityEditor.SceneView.currentDrawingSceneView.camera;
        float scaleFactor = 1f / mainCamera.orthographicSize;

        UnityEditor.Handles.color = _colorNumbers;
        GUIStyle style = new GUIStyle();
        style.normal.textColor = _colorNumbers;
        style.fontStyle = FontStyle.Bold;
        style.alignment = TextAnchor.UpperCenter;
        style.fontSize = Mathf.RoundToInt(_fontSize * scaleFactor);

        Gizmos.color = _colorGrid;
        for (float i = 0; i < 50; i += 1.20f)
        {
            Gizmos.DrawLine(new Vector3(i, -2.75f - _lineHeight, 0), new Vector3(i, -2.75f + _lineHeight, 0));
            UnityEditor.Handles.Label(new Vector3(i, -3, 0.0f),
                Mathf.Round(i * 100).ToString(CultureInfo.InvariantCulture),
                style);
        }

        Gizmos.color = _colorGrid;
        for (float i = 0; i > -50; i -= 1.20f)
        {
            Gizmos.DrawLine(new Vector3(i, -2.75f - _lineHeight, 0), new Vector3(i, -2.75f + _lineHeight, 0));
            UnityEditor.Handles.Label(new Vector3(i, -3, 0.0f),
                Mathf.Round(i * 100).ToString(CultureInfo.InvariantCulture),
                style);
        }
#endif
    }
}