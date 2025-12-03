using UnityEngine;

namespace Bonfire.Builds
{
    public class WoodBuildBonfire : MonoBehaviour
    {
        [SerializeField] private BonfireInfo _bonfireInfo;
        [SerializeField] private GameObject[] _woodPrefabs;

        [SerializeField] private float _avaliableWidth;
        [SerializeField] private float _avaliableHeight;

        [SerializeField] private float[] _randomXValues;
        [SerializeField] private float[] _randomYValues;

        private float _towerWidth;

        private float minX = float.MaxValue;
        private float maxX = float.MinValue;

        public void Initialize(SpriteRenderer spriteRenderer)
        {
            Bounds bounds = spriteRenderer.sprite.bounds;

            StartBuildingWoods(bounds.size, spriteRenderer.size.y);
        }


        private void StartBuildingWoods(Vector3 targerVectorSize, float height)
        {
            float leftBoundX = -targerVectorSize.x / 2;
            float rightBoundX = targerVectorSize.x / 2;

            for (float x = leftBoundX; x < rightBoundX; x += _avaliableWidth)
            {
                for (float y = 0; y < height; y += _avaliableHeight)
                    InstantiateWood(x, y);
            }

            transform.localPosition = new Vector3(GetCenterOfTheBuild(), 0, 0);
            _bonfireInfo.SortWood();
        }

        private float GetCenterOfTheBuild() =>
            (minX + maxX) / 2 * -1;

        private void InstantiateWood(float x, float y)
        {
            GameObject woodObject = Instantiate(_woodPrefabs[Random.Range(0, _woodPrefabs.Length)], transform);
            woodObject.SetActive(false);
            _bonfireInfo.WoodsList.Add(woodObject);

            woodObject.transform.localPosition = new Vector3(x, y, 0);
            woodObject.transform.localScale = new Vector3(_randomXValues[Random.Range(0, _randomXValues.Length)],
                _randomYValues[Random.Range(0, _randomYValues.Length)], 1);

            minX = Mathf.Min(minX, woodObject.transform.localPosition.x);
            maxX = Mathf.Max(maxX, woodObject.transform.localPosition.x);
        }
    }
}