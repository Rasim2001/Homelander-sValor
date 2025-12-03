using System;
using BuildProcessManagement;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Bonfire.Builds
{
    public class ScaffoldBuildBonfire : MonoBehaviour
    {
        [SerializeField] private BonfireInfo _bonfireInfo;
        [SerializeField] private GameObject[] _scaffoldsPrefabs;
        [SerializeField] private float _distanceBetweenScaffolds;

        private readonly int recursionDepthLimit = 100;

        public void Initialize(SpriteRenderer spriteRenderer)
        {
            Bounds bounds = spriteRenderer.sprite.bounds;

            StartBuildingScaffolds(bounds.size, bounds.center, spriteRenderer.size.y);
        }


        public void BuildScaffolds(BonfireInfo bonfireInfo)
        {
            Sequence sequence = DOTween.Sequence();
            for (int i = 0; i < bonfireInfo.ScaffoldsList.Count; i++)
            {
                GameObject gameObject = bonfireInfo.ScaffoldsList[i];

                if (gameObject.activeInHierarchy)
                    return;

                gameObject.SetActive(true);
                SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

                Tween colorTween = spriteRenderer.DOColor(Color.white, 0.1f);
                sequence.Append(colorTween);
            }

            sequence.Play();
        }

        private void StartBuildingScaffolds(Vector3 targerVectorSize, Vector3 worldCenter, float height)
        {
            float leftBoundX = -targerVectorSize.x / 2;
            float rightBoundX = targerVectorSize.x / 2;
            float centerBoundX = worldCenter.x;

            InstantiateScaffoldPiece(leftBoundX, height);
            InstantiateScaffoldPiece(rightBoundX, height);
            InstantiateScaffoldPiece(centerBoundX, height);

            CreatePieceOfScaffoldInBetween(leftBoundX, centerBoundX, height);
            CreatePieceOfScaffoldInBetween(centerBoundX, rightBoundX, height);

            _bonfireInfo.SortScaffold();
        }

        private void CreatePieceOfScaffoldInBetween(float minBoundX, float maxBoundX, float height, int depth = 0)
        {
            if (depth > recursionDepthLimit)
                throw new Exception("Превышена глубина");

            float center = (maxBoundX + minBoundX) / 2f;

            if (Mathf.Abs(maxBoundX - minBoundX) > _distanceBetweenScaffolds)
            {
                InstantiateScaffoldPiece(center, height);

                CreatePieceOfScaffoldInBetween(minBoundX, center, depth + 1);
                CreatePieceOfScaffoldInBetween(center, maxBoundX, depth + 1);
            }
        }


        private void InstantiateScaffoldPiece(float boundX, float height)
        {
            GameObject pieceOfScaffolds = Instantiate(
                _scaffoldsPrefabs[Random.Range(0, _scaffoldsPrefabs.Length)], transform);

            SpriteRenderer spriteRenderer = pieceOfScaffolds.GetComponent<SpriteRenderer>();
            spriteRenderer.size = new Vector2(spriteRenderer.size.x, height);

            pieceOfScaffolds.transform.localPosition = new Vector3(boundX, 0, 0);

            _bonfireInfo.ScaffoldsList.Add(pieceOfScaffolds);
            pieceOfScaffolds.SetActive(false);
        }
    }
}