using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Tooltip.World
{
    public class TooltipTest : MonoBehaviour
    {
        [SerializeField] private TextMeshPro textMeshPro;

        [Button]
        private void Check()
        {
            textMeshPro.ForceMeshUpdate();

            // Получаем границы текста
            Bounds bounds = textMeshPro.textBounds;
            Vector3 size = bounds.size;

            Debug.Log($"Ширина: {size.x}, Высота: {size.y}, Глубина: {size.z}");
        }

        private void OnMouseEnter()
        {
            Debug.Log("Enter");
        }

        private void OnMouseExit()
        {
            Debug.Log("Exit");
        }
    }
}