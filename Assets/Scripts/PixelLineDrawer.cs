using UnityEngine;

public class PixelLineDrawer : MonoBehaviour
{
    [Range(0, 25)] [SerializeField] private float _lineHeight;
    [SerializeField] private Color _colorBetweenLines;

    private void OnDrawGizmos()
    {
        Gizmos.color = _colorBetweenLines;

        for (float i = 0.60f; i < 50; i += 1.20f)
            Gizmos.DrawLine(new Vector3(i, -2.75f - _lineHeight, 0), new Vector3(i, -2.75f + _lineHeight, 0));

        for (float i = -0.60f; i > -50; i -= 1.20f)
            Gizmos.DrawLine(new Vector3(i, -2.75f - _lineHeight, 0), new Vector3(i, -2.75f + _lineHeight, 0));
    }
}