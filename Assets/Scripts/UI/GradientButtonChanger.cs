using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class GradientButtonChanger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private TextMeshProUGUI _textMeshPro;

        private readonly Color _startColor = new Color(168f / 255, 137f / 255, 79f / 255, 1);
        private readonly Color _endColor = new Color(1, 227f / 255, 173f / 255, 1);

        private Color _defaultColor;

        private void Awake() =>
            _defaultColor = _textMeshPro.color;

        public void OnPointerEnter(PointerEventData eventData) =>
            ApplyGradient();

        public void OnPointerExit(PointerEventData eventData) =>
            ResetTextColor();

        private void ApplyGradient()
        {
            if (_textMeshPro == null)
                return;

            _textMeshPro.ForceMeshUpdate(); // Обновляем текст, чтобы получить доступ к вершинам
            TMP_TextInfo textInfo = _textMeshPro.textInfo;

            if (textInfo == null)
                return;

            for (int i = 0; i < textInfo.characterCount; i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                if (!charInfo.isVisible) continue;

                int vertexIndex = charInfo.vertexIndex;
                Color32[] vertexColors = textInfo.meshInfo[charInfo.materialReferenceIndex].colors32;

                float gradientRatio = (float)i / (textInfo.characterCount - 1);
                Color32 color = Color32.Lerp(_startColor, _endColor, gradientRatio);

                vertexColors[vertexIndex + 0] = color;
                vertexColors[vertexIndex + 1] = color;
                vertexColors[vertexIndex + 2] = color;
                vertexColors[vertexIndex + 3] = color;
            }

            _textMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
        }

        private void ResetTextColor()
        {
            if (_textMeshPro == null)
                return;

            _textMeshPro.ForceMeshUpdate();
            TMP_TextInfo textInfo = _textMeshPro.textInfo;

            // Цвет, к которому нужно вернуть все вершины
            Color32 defaultColor = _defaultColor;

            for (int i = 0; i < textInfo.characterCount; i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                if (!charInfo.isVisible) continue;

                int vertexIndex = charInfo.vertexIndex;
                Color32[] vertexColors = textInfo.meshInfo[charInfo.materialReferenceIndex].colors32;

                vertexColors[vertexIndex + 0] = defaultColor;
                vertexColors[vertexIndex + 1] = defaultColor;
                vertexColors[vertexIndex + 2] = defaultColor;
                vertexColors[vertexIndex + 3] = defaultColor;
            }

            _textMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
        }
    }
}