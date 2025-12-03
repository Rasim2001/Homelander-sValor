using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Infastructure.Services.VagabondCampManagement;
using Infastructure.StaticData.Forest;
using Infastructure.StaticData.StaticDataService;
using Units.Vagabond;
using UnityEngine;
using Zenject;

namespace Infastructure.Services.Forest
{
    public class ForestService : IForestService, IInitializable
    {
        private const string Forestcontainer = "ForestContainer";

        private readonly List<GameObject> _leftForest = new();
        private readonly List<GameObject> _rightForest = new();

        private readonly IStaticDataService _staticData;
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly IVagabondCampService _vagabondCampService;

        private Transform _forestContainer;

        public ForestService(IStaticDataService staticData, ICoroutineRunner coroutineRunner,
            IVagabondCampService vagabondCampService)
        {
            _staticData = staticData;
            _coroutineRunner = coroutineRunner;
            _vagabondCampService = vagabondCampService;
        }


        public void Initialize()
        {
            _forestContainer = new GameObject(Forestcontainer).transform;

            List<ForestStaticData> forestDatas = _staticData.ForestStaticDatas;
            List<ForestData> forestMarkers = _staticData.GameStaticData.ForestSides;

            foreach (ForestData forestMarker in forestMarkers)
            {
                List<GameObject> targetForest = forestMarker.Position.x > 0 ? _rightForest : _leftForest;

                foreach (ForestStaticData forestStaticData in forestDatas)
                {
                    List<GameObject> localForest = GenerateForest(forestStaticData, forestMarker);
                    targetForest.AddRange(localForest);
                }
            }
        }

        public void DestroyForest(float positionX, float positionNextResourceX)
        {
            bool isRightSide = positionX > 0;

            List<GameObject> targetForest = isRightSide ? _rightForest : _leftForest;
            List<GameObject> destroyedObjects = new List<GameObject>();

            VagabondCamp vagabondCamp =
                _vagabondCampService.GetClosestVagabondCamp(isRightSide, positionX);

            bool campShouldDestroy = isRightSide
                ? vagabondCamp.transform.position.x < positionNextResourceX
                : vagabondCamp.transform.position.x > positionNextResourceX;

            if (campShouldDestroy)
                _vagabondCampService.RemoveCamp(vagabondCamp);


            targetForest.RemoveAll(forest =>
            {
                if (forest == null)
                    return false;

                bool shouldDestroy = isRightSide
                    ? forest.transform.position.x < positionNextResourceX
                    : forest.transform.position.x > positionNextResourceX;

                if (shouldDestroy)
                {
                    destroyedObjects.Add(forest);
                    return true;
                }

                return false;
            });

            _coroutineRunner.StartCoroutine(StartDestroyObjectCoroutine(destroyedObjects));
        }

        private IEnumerator StartDestroyObjectCoroutine(List<GameObject> destroyedObjects)
        {
            yield return new WaitForSeconds(Random.Range(2, 3));

            foreach (GameObject forest in destroyedObjects)
            {
                SpriteRenderer spriteRenderer = forest.GetComponent<SpriteRenderer>();

                Color initialColor = spriteRenderer.color;
                spriteRenderer
                    .DOColor(new Color(initialColor.r, initialColor.g, initialColor.b, 0), Random.Range(1, 2))
                    .OnComplete(() => { Object.Destroy(forest); });


                yield return new WaitForSeconds(Random.Range(2, 3));
            }
        }


        private List<GameObject> GenerateForest(ForestStaticData forestStaticData, ForestData forestData)
        {
            int treesPlaced = 0;
            int attempts = 0;
            int maxAttempts = forestStaticData.TreeCount * 2;

            Transform localContainer = Object.Instantiate(forestStaticData.Container, _forestContainer).transform;
            localContainer.name = forestStaticData.name;

            List<GameObject> localForest = new List<GameObject>();

            Vector2 areaSize = forestData.ColliderSize;
            Vector2 areaCenter = forestData.Position + forestData.ColliderOffset;

            while (treesPlaced < forestStaticData.TreeCount && attempts < maxAttempts)
            {
                float x = Random.Range(areaCenter.x - areaSize.x / 2f, areaCenter.x + areaSize.x / 2f);
                float y = Random.Range(areaCenter.y - areaSize.y / 2f, areaCenter.y + areaSize.y / 2f);
                Vector2 position = new Vector2(x, y);

                if (IsPositionValid(position, forestStaticData, localForest))
                {
                    GameObject treePrefab =
                        forestStaticData.TreePrefabs[Random.Range(0, forestStaticData.TreePrefabs.Length)];
                    GameObject newTree =
                        Object.Instantiate(treePrefab, position, Quaternion.identity, localContainer);
                    newTree.transform.localScale = new Vector3(Random.Range(0.8f, 1.2f), 1, 1.35f);

                    localForest.Add(newTree);
                    treesPlaced++;

                    if (Random.value < 0.3f)
                        SpawnCluster(position, forestStaticData, localForest, localContainer);
                }

                attempts++;
            }

            return localForest;
        }

        private void SpawnCluster(Vector2 center,
            ForestStaticData forestStaticData,
            List<GameObject> localForest,
            Transform localContainer)
        {
            int clusterSize = Random.Range(1, 4);
            for (int i = 0; i < clusterSize; i++)
            {
                float offsetX = Random.Range(0.1f, 0.5f);
                Vector2 offset = Random.insideUnitCircle * offsetX;
                Vector2 newPos = center + offset;

                if (IsPositionValid(newPos, forestStaticData, localForest))
                {
                    GameObject treePrefab =
                        forestStaticData.TreePrefabs[Random.Range(0, forestStaticData.TreePrefabs.Length)];
                    GameObject newTree =
                        Object.Instantiate(treePrefab, newPos, Quaternion.identity, localContainer);
                    newTree.transform.localScale = new Vector3(Random.Range(0.8f, 1.2f), 1, 1.35f);

                    localForest.Add(newTree);
                }
            }
        }

        private bool IsPositionValid(
            Vector2 position,
            ForestStaticData forestStaticData,
            List<GameObject> localForest)
        {
            foreach (GameObject tree in localForest)
            {
                Vector2 treePos = tree.transform.position;

                float distance = Mathf.Abs(position.x - treePos.x);

                if (distance < forestStaticData.MinDistanceBetweenTrees)
                    return false;
            }

            return true;
        }
    }
}