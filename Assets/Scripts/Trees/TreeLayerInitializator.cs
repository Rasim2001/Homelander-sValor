using Infastructure.Services.Trees;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Zenject;

namespace Trees
{
    public class TreeLayerInitializator : MonoBehaviour
    {
        [SerializeField] private Light2D _light;
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private ISortingLayerTrees _sortingLayerTrees;

        [Inject]
        public void Construct(ISortingLayerTrees sortingLayerTrees) =>
            _sortingLayerTrees = sortingLayerTrees;

        private void Start()
        {
            int layerIndex = _sortingLayerTrees.GenerateSortingLayerId();

            _light.lightOrder = layerIndex;
            _spriteRenderer.sortingOrder = layerIndex;
        }
    }
}